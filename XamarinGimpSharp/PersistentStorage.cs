// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2011 Maurits Rijk
//
// PersistentStorage.cs
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.
//

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gimp
{
  public sealed class PersistentStorage
  {
    readonly Plugin _plugin;
    readonly string _name;
    BinaryFormatter _formatter = new BinaryFormatter();

    public PersistentStorage(Plugin plugin)
    {
      _plugin = plugin;
      _name = plugin.Name;
    }

    public void SetData(VariableSet variables)
    {
      var stream = new MemoryStream();
      variables.ForEach(v => v.Serialize(_formatter, stream));
      if (stream.Length != 0)
	{
	  ProceduralDb.SetData(_name, stream.GetBuffer());
	}
    }

    public void SetData()
    {
      var memoryStream = new MemoryStream();

      var type = _plugin.GetType();

      foreach (var attribute in new SaveAttributeSet(type))
	{
	  var field = attribute.Field;
	  _formatter.Serialize(memoryStream, field.GetValue(_plugin));
	}

      if (memoryStream.Length != 0)
	{
	  ProceduralDb.SetData(_name, memoryStream.GetBuffer());
	}
    }

    public void GetData(VariableSet variables)
    {
      var data = ProceduralDb.GetData(_name);
      if (data != null)
	{
	  var stream = new MemoryStream(data);
	  variables.ForEach(v => v.Deserialize(_formatter, stream));
	}
    }

    public void GetData()
    {
      var data = ProceduralDb.GetData(_name);
      if (data != null)
	{
	  var memoryStream = new MemoryStream(data);
	  var type = _plugin.GetType();
	  
	  foreach (var attribute in new SaveAttributeSet(type))
	    {
	      var field = attribute.Field;
	      field.SetValue(_plugin, _formatter.Deserialize(memoryStream));
	    }
	}
    }
  }
}
