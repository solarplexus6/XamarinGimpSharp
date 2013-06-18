// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2010 Maurits Rijk
//
// Rectangle.cs
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

namespace Gimp
{
  public class Rectangle
  {
      private readonly IntCoordinate _upperLeft;
      private readonly IntCoordinate _lowerRight;

    public Rectangle(int x1, int y1, int x2, int y2)
    {
      _upperLeft = new IntCoordinate(x1, y1);
      _lowerRight = new IntCoordinate(x2, y2);
    }

    public Rectangle(IntCoordinate upperLeft, int width, int height)
    {
      _upperLeft = upperLeft;
      _lowerRight = new IntCoordinate(upperLeft.X + width,
					upperLeft.Y + height);
    }

    public Rectangle(IntCoordinate upperLeft, IntCoordinate lowerRight)
    {
      _upperLeft = upperLeft;
      _lowerRight = lowerRight;
    }

    public int X1
    {
      get {return _upperLeft.X;}
    }

    public int Y1
    {
      get {return _upperLeft.Y;}
    }

    public int X2
    {
      get {return _lowerRight.X;}
    }

    public int Y2
    {
      get {return _lowerRight.Y;}
    }

    public IntCoordinate UpperLeft
    {
      get {return _upperLeft;}
    }

    public IntCoordinate LowerRight
    {
      get {return _lowerRight;}
    }

    public int Width
    {
      get {return _lowerRight.X - _upperLeft.X;}
    }

    public int Height
    {
      get {return _lowerRight.Y - _upperLeft.Y;}
    }

    public int Area
    {
      get {return Width * Height;}
    }

    public override bool Equals(object o)
    {
      if (o == null)
      {
          return false;
      }
      if (o is Rectangle)
	{
	  var rectangle = o as Rectangle;	    
	  return rectangle.UpperLeft == UpperLeft &&
	    rectangle.LowerRight == LowerRight;
	}
      return false;
    }

    public override int GetHashCode()
    {
      return _upperLeft.GetHashCode() + _lowerRight.GetHashCode();
    }

    public static bool operator==(Rectangle rectangle1, Rectangle rectangle2)
    {        
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(rectangle1, rectangle2))
        {
            return true;
        }        
        // If one is null, but not both, return false.
        if (((object)rectangle1 == null) || ((object)rectangle2 == null))
        {
            return false;
        }
        
      return rectangle1.Equals(rectangle2);
    }

    public static bool operator!=(Rectangle rectangle1, Rectangle rectangle2)
    {
      return !(rectangle1 == rectangle2);
    }

    public override string ToString()
    {
      return string.Format("({0}, {1}, {2}, {3})", X1, Y1, X2, Y2);
    }
  }
}
