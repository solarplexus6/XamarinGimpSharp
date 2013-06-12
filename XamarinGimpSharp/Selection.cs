// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2009 Maurits Rijk
//
// Selection.cs
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
using System.Runtime.InteropServices;

namespace Gimp
{
  public sealed class Selection : Channel
  {
    readonly Int32 _imageID;

    internal Selection(Int32 imageID, Int32 selectionID) : base(selectionID)
    {
      _imageID = imageID;
    }

      /// <summary>
      /// Find the bounding box of the current selection
      /// </summary>
      /// <param name="nonEmpty"></param>
      /// <param name="x1"></param>
      /// <param name="y1"></param>
      /// <param name="x2"></param>
      /// <param name="y2"></param>
    public new void Bounds(out bool nonEmpty,
			   out int x1, out int y1,
			   out int x2, out int y2)
    {
      if (!gimp_selection_bounds(_imageID, out nonEmpty,
				 out x1, out y1, out x2, out y2))
        {
	  throw new GimpSharpException();
        }
    }

    public new Rectangle Bounds(out bool nonEmpty)
    {
      int x1, y1, x2, y2;
      Bounds(out nonEmpty, out x1, out y1, out x2, out y2);
      return new Rectangle(x1, y1, x2, y2);
    }

    public void All()
    {
      if (!gimp_selection_all(_imageID))
        {
	  throw new GimpSharpException();
        }
    }

    public void None()
    {
      if (!gimp_selection_none(_imageID))
        {
	  throw new GimpSharpException();
        }
    }

    public bool Empty
    {
      get {return gimp_selection_is_empty(_imageID);}
    }

    public Layer Float(Drawable drawable, int offx, int offy)
    {
      return new Layer(gimp_selection_float(_imageID,
					    drawable.ID, offx, offy));
    }

    public void Load(Channel channel)
    {
      if (!gimp_selection_load(channel.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public Channel Save()
    {
      return new Channel(gimp_selection_save(_imageID));
    }

    public int Value(int x, int y)
    {
      return gimp_selection_value(_imageID, x, y);
    }

    public int this[int x, int y]
    {
      get {return Value(x, y);}
    }

    public void Grow(int steps)
    {
      if (!gimp_selection_grow(_imageID, steps))
        {
	  throw new GimpSharpException();
        }
    }

    public void Shrink(int radius)
    {
      if (!gimp_selection_shrink(_imageID, radius))
        {
	  throw new GimpSharpException();
        }
    }

    public new void Invert()
    {
      if (!gimp_selection_invert(_imageID))
        {
	  throw new GimpSharpException();
        }
    }

    public void Feather(double radius)
    {
      if (!gimp_selection_feather(_imageID, radius))
        {
	  throw new GimpSharpException();
        }
    }

    public void Sharpen()
    {
      if (!gimp_selection_sharpen(_imageID))
        {
	  throw new GimpSharpException();
        }
    }

    public void Border(int radius)
    {
      if (!gimp_selection_border(_imageID, radius))
        {
	  throw new GimpSharpException();
        }
    }

    public void Translate(int offx, int offy)
    {
      if (!gimp_selection_translate(_imageID, offx, offy))
        {
	  throw new GimpSharpException();
        }
    }

    public void Translate(Offset offset)
    {
      Translate(offset.X, offset.Y);
    }

    public void LayerAlpha(Layer layer)
    {
      if (!gimp_selection_layer_alpha(layer.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void Combine(Channel channel, ChannelOps operation)
    {
      if (!gimp_selection_combine(channel.ID, operation))
        {
	  throw new GimpSharpException();
        }
    }

    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_bounds (Int32 image_ID,
                                              out bool non_empty,
                                              out int x1,
                                              out int y1,
                                              out int x2,
                                              out int y2);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_all (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_none (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_is_empty (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static Int32 gimp_selection_float (Int32 image_ID,
                                              Int32 drawable_ID,
                                              int offx, int offy);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_load (Int32 channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static Int32 gimp_selection_save (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static int gimp_selection_value (Int32 image_ID,
                                            int x, int y);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_grow (Int32 image_ID,
                                            int steps);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_shrink (Int32 image_ID,
                                              int radius);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_invert (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_feather (Int32 image_ID,
                                               double radius);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_sharpen (Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_border (Int32 image_ID,
                                              int radius);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_translate (Int32 image_ID,
                                                 int offx, int offy);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_layer_alpha (Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_selection_combine (Int32 channel_ID,
                                               ChannelOps operation); 
  }
}
