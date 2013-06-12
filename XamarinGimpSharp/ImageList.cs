// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2009 Maurits Rijk
//
// ImageList.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Gimp
{
  public sealed class ImageList
  {
    readonly List<Image> _list = new List<Image>();

    public ImageList()
    {
      Refresh();
    }

    public void Refresh()
    {
      _list.Clear();
      int numImages;
      IntPtr list = gimp_image_list(out numImages);

      if (numImages != 0)
	{
	  var dest = new int[numImages];
	  Marshal.Copy(list, dest, 0, numImages);
	  Array.ForEach(dest, imageID => _list.Add(new Image(imageID)));
	}
    }

    public IEnumerator<Image> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    public void ForEach(Action<Image> action)
    {
      _list.ForEach(action);
    }

    public int Count
    {
      get {return _list.Count;}
    }

    public Image this[int index]
    {
      get {return _list[index];}
    }

    public int GetIndex(Image key)
    {
      return _list.FindIndex(image => image.ID == key.ID);
    }

    [DllImport("libgimp-2.0-0.dll")]
    static extern IntPtr gimp_image_list(out int num_images);
  }
}
