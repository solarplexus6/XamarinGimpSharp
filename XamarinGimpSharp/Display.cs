// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2010 Maurits Rijk
//
// Display.cs
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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gimp
{
  public sealed class Display
  {
    readonly Int32 _displayID = -1;

    public Display(Image image)
    {
      Debug.Assert(image != null);
      _displayID = gimp_display_new(image.ID);
    }

    public void Delete()
    {
      Debug.Assert(_displayID != -1);
      if (!gimp_display_delete(_displayID))
        {
	  throw new GimpSharpException();
        }
    }

    public bool Valid
    {
      get {return gimp_display_is_valid(_displayID);}
    }

    public int WindowHandle
    {
      get {return gimp_display_get_window_handle(_displayID);}
    }

    public static void DisplaysFlush()
    {
      gimp_displays_flush();
    }

    public static bool Reconnect(Image oldImage, Image newImage)
    {
      Debug.Assert(oldImage != null && newImage != null);
      return gimp_displays_reconnect(oldImage.ID, newImage.ID);
    }

    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_display_new(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_display_delete(Int32 display_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_display_is_valid(Int32 display_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_display_get_window_handle(Int32 display_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern void gimp_displays_flush();
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_displays_reconnect(Int32 old_image_ID,
                                               Int32 new_image_ID);
  }
}
