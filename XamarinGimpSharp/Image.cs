// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2010 Maurits Rijk
//
// Image.cs
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

using Gdk;
using GLib;

namespace Gimp
{
  public sealed class Image : IEnumerable
  {
    internal Int32 _imageID;
     
    internal Image(Int32 imageID)
    {
      ID = imageID;
    }
       
    public Image(int width, int height, ImageBaseType type)
    {
      ID = gimp_image_new(width, height, type);
    }

    public Image(Dimensions dimensions, ImageBaseType type) :
      this(dimensions.Width, dimensions.Height, type)
    {
    }

    public Image(Image image)
    {
      ID = gimp_image_duplicate(image._imageID);
    }

    public Image Duplicate()
    {
      return new Image(this);
    }

    public override bool Equals(object o)
    {
      if (o is Image)
	{
	  return ID == (o as Image).ID;
	}
      return false;
    }

    public override int GetHashCode()
    {
      return ID.GetHashCode();
    }

    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public static Image Load(RunMode runMode, string filename, 
                             string raw_filename)
    {
      Int32 imageID = gimp_file_load(runMode, filename, raw_filename);
      return (imageID >= 0) ? new Image(imageID) : null;
    }

    public Layer LoadLayer(RunMode runMode, string filename)
    {
      return new Layer(gimp_file_load_layer(runMode, ID, filename));
    }

    public LayerList LoadLayers(RunMode runMode, string filename)
    {
      int numLayers;
      IntPtr ptr = gimp_file_load_layers(runMode, ID, filename, 
					 out numLayers);
      return new LayerList(ptr, numLayers);
    }

    public bool Save(RunMode run_mode, string filename, string raw_filename)
    {
      Int32 drawableID = ActiveDrawable.ID;	// Check this!
      return gimp_file_save(run_mode, ID, drawableID, filename,
                            raw_filename);
    }

    // Used internally to save a single drawable
    internal bool Save(Drawable drawable, string filename)
    {
      return gimp_file_save(RunMode.Noninteractive, ID, drawable.ID, 
			    filename, filename);
    }

    public void Delete()
    {
      if (!gimp_image_delete(ID))
        {
	  throw new GimpSharpException();
        }
    }
    
    public bool IsValid
    {
      get {return gimp_image_is_valid(ID);}
    }

    public ImageBaseType BaseType
    {
      get {return gimp_image_base_type(ID);}
    }

    public int Width
    {
      get {return gimp_image_width(ID);}
    }

    public int Height
    {
      get {return gimp_image_height(ID);}
    }

    public Rectangle Bounds
    {
      get {return new Rectangle(0, 0, Width, Height);}
    }

    public Dimensions Dimensions
    {
      get {return new Dimensions(Width, Height);}
    }

    public void Flip(OrientationType flip_type)
    {
      if (!gimp_image_flip(ID, flip_type))
        {
	  throw new GimpSharpException();
        }
    }

    public void Rotate(RotationType rotate_type)
    {
      if (!gimp_image_rotate(ID, rotate_type))
        {
	  throw new GimpSharpException();
        }
    }

    public void Resize(int newWidth, int newHeight, int offX, int offY)
    {
      if (!gimp_image_resize(ID, newWidth, newHeight, offX, offY))
        {
	  throw new GimpSharpException();
        }
    }

    public void Resize(Dimensions dimensions, Offset offset)
    {
      Resize(dimensions.Width, dimensions.Height, offset.X, offset.Y);
    }

    public void ResizeToLayers()
    {
      if (!gimp_image_resize_to_layers(ID))
        {
	  throw new GimpSharpException();
        }
    }
       
    public void Scale(int newWidth, int newHeight)
    {
      if (!gimp_image_scale(ID, newWidth, newHeight))
        {
	  throw new GimpSharpException();
        }
    }

    public void Scale(Dimensions dimensions)
    {
      Scale(dimensions.Width, dimensions.Height);
    }

    public void Scale(int newWidth, int newHeight, 
		      InterpolationType interpolation)
    {
      if (!gimp_image_scale_full(ID, newWidth, newHeight, interpolation))
        {
	  throw new GimpSharpException();
        }
    }

    public void Scale(Dimensions dimensions, InterpolationType interpolation)
    {
      Scale(dimensions.Width, dimensions.Height, interpolation);
    }

    public void Crop(int newWidth, int newHeight, int offx, int offy)
    {
      if (!gimp_image_crop(ID, newWidth, newHeight, offx, offy))
        {
	  throw new GimpSharpException();
        }
    }

    public void Crop(Dimensions dimensions, Offset offset)
    {
      Crop(dimensions.Width, dimensions.Height, offset.X, offset.Y);
    }

    public void Crop(Rectangle rectangle)
    {
      Crop(rectangle.Width, rectangle.Height, rectangle.X1, rectangle.Y1);
    }
      
    public LayerList Layers
    {
      get {return new LayerList(this);}
    }

    // Beware: this ChannelList isn't updated when channels are added!
    public ChannelList Channels
    {
      get {return new ChannelList(this);}
    }

    public Drawable ActiveDrawable
    {
      get {return new Drawable(gimp_image_get_active_drawable(ID));}
    }

    public FloatingSelection FloatingSelection
    {
      get
	{
          Int32 layerID = gimp_image_get_floating_sel(ID);
          return (layerID == -1) ? null : new FloatingSelection(layerID);
	}
    }

    public Drawable FloatingSelectionAttachedTo
    {
      get
	{
          Int32 drawableID = gimp_image_floating_sel_attached_to(ID);
          return (drawableID == -1) ? null : new Drawable(drawableID);
	}
    }

    public RGB PickColor(Drawable drawable, Coordinate<double> c,
                         bool sampleMerged, bool sampleAverage, 
			 double averageRadius)
    {
      GimpRGB color;
      if (!gimp_image_pick_color(ID, drawable.ID, c.X, c.Y, 
				 sampleMerged, sampleAverage, averageRadius, 
				 out color))
        {
	  return null;
        }
      return new RGB(color);
    }

    public Layer PickCorrelateLayer(Coordinate<int> c)
    {
      Int32 layerID = gimp_image_pick_correlate_layer(ID, c.X, c.Y);
      return (layerID == -1) ? null : new Layer(layerID);
    }

    public void AddLayer(Layer layer, int position)
    {
      if (!gimp_image_add_layer(ID, layer.ID, position))
        {
	  throw new GimpSharpException();
        }
    }

    public void Add(Layer layer, int position)
    {
      AddLayer(layer.DelayedConstruct(this), position);
    }

    public void RemoveLayer(Layer layer)
    {
      if (!gimp_image_remove_layer(ID, layer.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void RaiseLayer(Layer layer)
    {
      if (!gimp_image_raise_layer(ID, layer.ID))
        {
	  throw new GimpSharpException();
        }
    }
       
    public void LowerLayer(Layer layer)
    {
      if (!gimp_image_lower_layer(ID, layer.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void RaiseLayerToTop(Layer layer)
    {
      if (!gimp_image_raise_layer_to_top(ID, layer.ID))
        {
	  throw new GimpSharpException();
        }
    }
       
    public void LowerLayerToBottom(Layer layer)
    {
      if (!gimp_image_lower_layer_to_bottom(ID, layer.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public int GetLayerPosition(Layer layer)
    {
      return gimp_image_get_layer_position(ID, layer.ID);
    }

    public void AddChannel(Channel channel, int position)
    {
      if (!gimp_image_add_channel(ID, channel.ID, position))
        {
	  throw new GimpSharpException();			  
        }
    }

    public void AddChannel(Channel channel)
    {
      AddChannel(channel, 0);
    }

    public void RemoveChannel(Channel channel)
    {
      if (!gimp_image_remove_channel(ID, channel.ID))
        {
	  throw new GimpSharpException();			  
        }
    }

    public void RaiseChannel(Channel channel)
    {
      if (!gimp_image_raise_channel(ID, channel.ID))
        {
	  throw new GimpSharpException();			  
        }
    }
       
    public void LowerChannel(Channel channel)
    {
      if (!gimp_image_lower_channel(ID, channel.ID))
        {
	  throw new GimpSharpException();			  
        }
    }

    public int GetChannelPosition(Channel channel)
    {
      return gimp_image_get_channel_position(ID, channel.ID);
    }

    public Layer Flatten()
    {
      return new Layer(gimp_image_flatten(ID));
    }

    public Layer MergeVisibleLayers(MergeType merge_type)
    {
      return new Layer(gimp_image_merge_visible_layers(ID, merge_type));
    }

    public Layer MergeDown(Layer layer, MergeType merge_type)
    {
      return new Layer(gimp_image_merge_down(ID, layer.ID, merge_type));
    }

    public void CleanAll()
    {
      if (!gimp_image_clean_all(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public bool IsDirty
    {
      get {return gimp_image_is_dirty(ID);}
    }

    public Layer ActiveLayer
    {
      get {return new Layer(gimp_image_get_active_layer(ID));}
      set 
	{
          if (!gimp_image_set_active_layer(ID, value.ID))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public Channel ActiveChannel
    {
      get {return new Channel(gimp_image_get_active_channel(ID));}
      set 
	{
          if (!gimp_image_set_active_channel(ID, value.ID))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public void UnsetActiveChannel()
    {
      if (!gimp_image_unset_active_channel(ID))
	{
	  throw new GimpSharpException();
	}
    }

    public Selection Selection
    {
      get {return new Selection(ID, gimp_image_get_selection(ID));}
    }

    public bool GetComponentActive(ChannelType component)
    {
      return gimp_image_get_component_active(ID, component);
    }

    public void SetComponentActive(ChannelType component, bool active)
    {
      if (!gimp_image_set_component_active(ID, component, active))
        {
	  throw new GimpSharpException();
        }
    }

    public bool GetComponentVisible(ChannelType component)
    {
      return gimp_image_get_component_visible(ID, component);
    }

    public void SetComponentVisible(ChannelType component, bool visible)
    {
      if (!gimp_image_set_component_visible(ID, component, visible))
        {
	  throw new GimpSharpException();
        }
    }

    public string Filename
    {
      get {return gimp_image_get_filename(ID);}
      set
	{
          if (!gimp_image_set_filename(ID, value))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public string Name
    {
      get {return gimp_image_get_name(ID);}
    }

    public Resolution Resolution
    {
      get
	{
	  double xresolution, yresolution;
	  if (!gimp_image_get_resolution(ID, out xresolution, 
					 out yresolution))
	    {
	      throw new GimpSharpException();
	    }
	  return new Resolution(xresolution, yresolution);
	}
      set
	{
	  if (!gimp_image_set_resolution(ID, value.X, value.Y))
	    {
	      throw new GimpSharpException();
	    }
	}
    }

    public Unit Unit
    {
      get {return gimp_image_get_unit(ID);}
      set 
	{
          if (!gimp_image_set_unit(ID, value))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public int TattooState
    {
      get {return gimp_image_get_tattoo_state(ID);}
      set 
	{
          if (!gimp_image_set_tattoo_state(ID, value))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public Layer GetLayerByTattoo(Tattoo tattoo)
    {
      return new Layer(gimp_image_get_layer_by_tattoo(ID, tattoo.ID));
    }

    public Channel GetChannelByTattoo(Tattoo tattoo)
    {
      return new Channel(gimp_image_get_channel_by_tattoo(ID, 
                                                          tattoo.ID));
    }

    public RGB[] Colormap
    {
      get
	{
          int num_colors;
          IntPtr cmap = gimp_image_get_colormap(ID, out num_colors);
	  var rgb = new RGB[num_colors];
	  var tmp = new byte[3];

	  int index = 0;
	  for (int i = 0; i < num_colors; i++) 
	    {
	      Marshal.Copy(cmap, tmp, index, 3);
	      rgb[i] = new RGB(tmp[0], tmp[1], tmp[2]);
	      index += 3;
	    }

          Marshaller.Free(cmap);
          return rgb;
	}

      set
	{
	  int num_colors = value.Length;
	  var colormap = new byte[num_colors * 3];
	  int index = 0;
	  foreach (RGB rgb in value)
	    {
	      rgb.GetUchar(out colormap[index + 0],
			   out colormap[index + 1], 
			   out colormap[index + 2]);
	      index += 3;
	    }

          if (!gimp_image_set_colormap(ID, colormap, num_colors))
            {
	      throw new GimpSharpException();
            }
	}
    }

    public Pixel[,] GetThumbnail(Dimensions dimensions, Transparency alpha)
    {
      IntPtr src = gimp_image_get_thumbnail(ID, dimensions.Width,
					    dimensions.Height, alpha);
      var pixbuf = new Pixbuf(src);

      int bpp = ActiveDrawable.Bpp;
      var thumbnail = Pixel.ConvertToPixelArray(pixbuf.Pixels, dimensions, bpp);
      // Marshaller.Free(src);
      return thumbnail;
    }

    public List<Vectors> Vectors
    {
      get
	{
	  int numVectors;
	  IntPtr ptr = gimp_image_get_vectors(ID, out numVectors);
	  return GetVectorsFromIntPtr(ptr, numVectors);
	}
    }

    public Vectors GetVectorsByTattoo(Tattoo tattoo)
    {
      return new Vectors(gimp_image_get_vectors_by_tattoo(ID, tattoo.ID));
    }

    public Parasite ParasiteFind(string name)
    {
      IntPtr found = gimp_image_parasite_find(ID, name);
      return (found == IntPtr.Zero) ? null : new Parasite(found);
    }

    public void ParasiteAttach(Parasite parasite)
    {
      if (!gimp_image_parasite_attach(ID, parasite.Ptr))
        {
	  throw new GimpSharpException();
        }
    }

    public void ParasiteDetach(string name)
    {
      if (!gimp_image_parasite_detach(ID, name))
        {
	  throw new GimpSharpException();
        }
    }

    public void AttachNewParasite(string name, int flags, int size, object data)
    {
      if (!gimp_image_attach_new_parasite(ID, name, flags, size, data))
        {
	  throw new GimpSharpException();
        }
    }

    public void AddVectors(Vectors vectors, int position)
    {
      if (!gimp_image_add_vectors(ID, vectors.ID, position))
        {
	  throw new GimpSharpException();
        }
    }

    public void RemoveVectors(Vectors vectors)
    {
      if (!gimp_image_remove_vectors(ID, vectors.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public Vectors ActiveVectors
    {
      get
	{
	  int vectorsID = gimp_image_get_active_vectors(ID);
	  return (vectorsID == -1) ? null : new Vectors(vectorsID);
	}
      set
	{
	  if (!gimp_image_set_active_vectors(ID, value.ID))
	    {
	      throw new GimpSharpException();
	    }
	}
    }

    public void LowerVectors(Vectors vectors)
    {
      if (!gimp_image_lower_vectors(ID, vectors.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void RaiseVectors(Vectors vectors)
    {
      if (!gimp_image_raise_vectors(ID, vectors.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void LowerVectorsToBottom(Vectors vectors)
    {
      if (!gimp_image_lower_vectors_to_bottom(ID, vectors.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void RaiseVectorsToTop(Vectors vectors)
    {
      if (!gimp_image_raise_vectors_to_top(ID, vectors.ID))
        {
	  throw new GimpSharpException();
        }
    }

    public int GetVectorsPosition(Vectors vectors)
    {
      return gimp_image_get_vectors_position(ID, vectors.ID);
    }

    public void ConvertRgb()
    {
      if (!gimp_image_convert_rgb(ID))
	{
	  // throw new GimpSharpException();
	}
    }
       
    public void ConvertGrayscale()
    {
      if (!gimp_image_convert_grayscale(ID))
	{
	  throw new GimpSharpException();
	}
    }
       
       
    public void ConvertIndexed(ConvertDitherType dither_type,
                               ConvertPaletteType palette_type,
                               int num_cols,
                               bool alpha_dither,
                               bool remove_unused,
                               string palette)
    {
      if (!gimp_image_convert_indexed(ID,
				      dither_type,
				      palette_type,
				      num_cols,
				      alpha_dither,
				      remove_unused,
				      palette))
	{
	  // throw new GimpSharpException();
	}
    }

    [DllImport("libgimpui-2.0-0.dll")]
    static extern IntPtr gimp_image_get_thumbnail(Int32 image_ID,
						  int width,
						  int height,
						  Transparency alpha);
    
    public Pixbuf GetThumbnail (int width, int height, Transparency alpha)
    {
      return new Pixbuf(gimp_image_get_thumbnail(ID, width, height, alpha));
    }       

    public void UndoGroupStart()
    {
      if (!gimp_image_undo_group_start(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public bool UndoEnabled
    {
      get {return gimp_image_undo_is_enabled(ID);}
    }

    public void UndoGroupEnd()
    {
      if (!gimp_image_undo_group_end(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void UndoDisable()
    {
      if (!gimp_image_undo_disable(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void UndoEnable()
    {
      if (!gimp_image_undo_enable(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void UndoFreeze()
    {
      if (!gimp_image_undo_freeze(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void UndoThaw()
    {
      if (!gimp_image_undo_thaw(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public void EditCopyVisible()
    {
      if (!gimp_edit_copy_visible(ID))
        {
	  throw new GimpSharpException();
        }
    }

    public List<Vectors>
    ImportVectorsFromFile(string filename, bool merge, bool scale)
    {
      var vectors = new List<Vectors>();
      int numVectors;
      IntPtr vectorsIds;

      if (!gimp_vectors_import_from_file(ID, filename, merge, scale,
					 out numVectors,
					 out vectorsIds))
	{
	  throw new GimpSharpException();
	}
      return vectors;
    }

    public List<Vectors>
    ImportVectorsFromString(string source, bool merge, bool scale)
    {
      int numVectors;
      IntPtr vectorsIds;

      if (!gimp_vectors_import_from_string(ID, source, -1, merge, scale,
					   out numVectors,
					   out vectorsIds))
	{
	  throw new GimpSharpException();
	}
      return GetVectorsFromIntPtr(vectorsIds, numVectors);
    }

    List<Vectors> GetVectorsFromIntPtr(IntPtr vectorsIds, int numVectors)
    {
      var vectors = new List<Vectors>();
      if (numVectors > 0)
	{
	  var dest = new int[numVectors];
	  Marshal.Copy(vectorsIds, dest, 0, numVectors);
	  Array.ForEach(dest, 
			vectorsID => vectors.Add(new Vectors(vectorsID)));
	}
      return vectors;
    }

    // Misc functions

    internal Int32 ID
    {
      get {return _imageID;}
      set {_imageID = value;}
    }
       
    public GuideCollection Guides
    {
      get {return new GuideCollection(this);}
    }

    public Grid Grid
    {
      get {return new Grid(ID);}
    }

    public override string ToString()
    {
      return "Image: " + ID;
    }

    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_new(int width, int height, 
				       ImageBaseType type);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_duplicate(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_delete(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_is_valid(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern ImageBaseType gimp_image_base_type(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_width(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_height(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_flip(Int32 image_ID,
				       OrientationType flip_type);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_rotate(Int32 image_ID,
					 RotationType rotate_type);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_resize(Int32 image_ID,
					 int new_width,
					 int new_height,
					 int offx,
					 int offy);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_resize_to_layers(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_scale(Int32 image_ID,
					int new_width,
					int new_height);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_scale_full(Int32 image_ID,
					     int new_width,
					     int new_height,
					     InterpolationType interpolation);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_crop(Int32 image_ID,
				       int new_width, int new_height,
				       int offx, int offy);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_active_drawable(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_floating_sel(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_floating_sel_attached_to(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool  gimp_image_pick_color(Int32 image_ID,
					      Int32 drawable_ID,
					      double x, double y,
					      bool sample_merged, 
					      bool sample_average,
					      double average_radius,
					      out GimpRGB color);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_pick_correlate_layer(Int32 image_ID,
							int x, int y);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_add_layer(Int32 image_ID,
					    Int32 layer_ID,
					    int position);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_remove_layer(Int32 image_ID,
					       Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_raise_layer(Int32 image_ID,
					      Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_lower_layer(Int32 image_ID,
					      Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_raise_layer_to_top(Int32 image_ID,
						     Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_lower_layer_to_bottom(Int32 image_ID,
							Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_get_layer_position(Int32 image_ID,
						    Int32 layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_add_channel(Int32 image_ID,
					      Int32 channel_ID,
					      int position);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_remove_channel(Int32 image_ID,
						 Int32 channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_raise_channel(Int32 image_ID,
						Int32 channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_lower_channel(Int32 image_ID,
						Int32 channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_get_channel_position(Int32 image_ID,
						      Int32 channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_flatten(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_merge_visible_layers(Int32 image_ID,
                                                        MergeType merge_type);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_merge_down(Int32 image_ID,
                                              Int32 merge_layer_ID,
                                              MergeType merge_type);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_clean_all(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_is_dirty(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_active_layer(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_active_layer(Int32 image_ID,
						   Int32 active_layer_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_active_channel(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_active_channel(Int32 image_ID,
						     Int32 active_channel_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_unset_active_channel(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_selection(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_get_component_active(Int32 image_ID,
						       ChannelType component);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_component_active(Int32 image_ID,
						       ChannelType component,
						       bool active);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_get_component_visible(Int32 image_ID,
							ChannelType component);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_component_visible(Int32 image_ID,
							ChannelType component,
							bool visible);
    [DllImport("libgimp-2.0-0.dll")]
    static extern string gimp_image_get_filename(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_filename(Int32 image_ID, 
					       string filename);
    [DllImport("libgimp-2.0-0.dll")]
    static extern string gimp_image_get_name(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_get_resolution(Int32 image_ID,
						 out double xresolution, 
						 out double yresolution);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_resolution(Int32 image_ID, 
						 double xresolution, 
						 double yresolution);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Unit gimp_image_get_unit(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_unit(Int32 image_ID, Unit unit);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_tattoo_state(Int32 image_ID, 
						   int tattoo_state);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_get_tattoo_state(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_layer_by_tattoo(Int32 image_ID, 
						       int tattoo);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_channel_by_tattoo(Int32 image_ID, 
							 int tattoo);
    [DllImport("libgimp-2.0-0.dll")]
    static extern IntPtr gimp_image_get_colormap(Int32 image_ID, 
                                                 out int num_colors);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_colormap(Int32 image_ID, 
					       byte[] colormap,
                                               int num_colors);
    [DllImport("libgimp-2.0-0.dll")]
    static extern IntPtr gimp_image_get_vectors(Int32 image_ID,
						out int num_vectors);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_vectors_by_tattoo(Int32 image_ID,
                                                         int tattoo);
    [DllImport("libgimp-2.0-0.dll")]
    static extern IntPtr gimp_image_parasite_find(Int32 image_ID,
                                                  string name);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_parasite_attach(Int32 image_ID,
                                                  IntPtr parasite);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_parasite_detach(Int32 imagee_ID,
                                                  string name);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_attach_new_parasite(Int32 image_ID,
                                                      string name, 
                                                      int flags,
                                                      int size,
                                                      object data);

    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_add_vectors(Int32 image_ID,
					      Int32 vectors_ID,
					      int position);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_remove_vectors(Int32 image_ID,
						 Int32 vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_image_get_active_vectors(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_set_active_vectors(Int32 image_ID,
						     Int32 active_vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_lower_vectors(Int32 image_ID,
						Int32 vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_raise_vectors(Int32 image_ID,
						Int32 vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_lower_vectors_to_bottom(Int32 image_ID,
							  Int32 vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_raise_vectors_to_top(Int32 image_ID,
						       Int32 vectors_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern int gimp_image_get_vectors_position(Int32 image_ID,
						      Int32 vectors_ID);

    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_convert_rgb(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_convert_grayscale(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_convert_indexed(Int32 image_ID,
						ConvertDitherType dither_type,
						ConvertPaletteType palette_type,
						  int num_cols,
						  bool alpha_dither,
						  bool remove_unused,
						  string palette);
    [DllImport("libgimp-2.0-0.dll")]
    static extern Int32 gimp_file_load(RunMode run_mode, string filename,
                                       string raw_filename);
    [DllImport("libgimp-2.0-0.dll")]
      static extern Int32 gimp_file_load_layer(RunMode run_mode, 
					       Int32 image_ID,
					       string filename);
    [DllImport("libgimp-2.0-0.dll")]
      static extern IntPtr gimp_file_load_layers(RunMode run_mode, 
						Int32 image_ID,
						string filename,
						 out int num_layers);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_file_save(RunMode run_mode, Int32 image_ID,
                                      Int32 drawable_ID, string filename,
                                      string raw_filename);

    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_group_start(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_group_end(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_is_enabled(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_disable(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_enable(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_freeze(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_image_undo_thaw(Int32 image_ID);

    [DllImport("libgimp-2.0-0.dll")]
    static extern bool gimp_edit_copy_visible(Int32 image_ID);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_vectors_import_from_file(Int32 image_ID,
						     string filename,
						     bool merge, bool scale,
						     out int num_vectors,
						     out IntPtr vectors_ids);
    [DllImport("libgimp-2.0-0.dll")]
    extern static bool gimp_vectors_import_from_string(Int32 image_ID,
						       string source,
						       int length,
						       bool merge, bool scale,
						       out int num_vectors,
						       out IntPtr vectors_ids);
  }
}
