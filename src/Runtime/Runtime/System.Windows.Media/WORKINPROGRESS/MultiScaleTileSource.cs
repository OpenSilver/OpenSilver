using System.Collections.Generic;

namespace System.Windows.Media
{
	//
	// Summary:
	//     Used to specify the source of Deep Zoom images.
    [OpenSilver.NotImplemented]
	public abstract partial class MultiScaleTileSource : DependencyObject
	{
		//
		// Summary:
		//     Creates a new instance of the System.Windows.Media.MultiScaleTileSource class
		//     with the specified parameters.
		//
		// Parameters:
		//   imageWidth:
		//     The width of the Deep Zoom image.
		//
		//   imageHeight:
		//     The height of the Deep Zoom image.
		//
		//   tileWidth:
		//     The width of the tiles in the Deep Zoom image.
		//
		//   tileHeight:
		//     The height of the tiles in the Deep Zoom image.
		//
		//   tileOverlap:
		//     How much the tiles in the Deep Zoom image overlap.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     One or more arguments are invalid.
        [OpenSilver.NotImplemented]
		public MultiScaleTileSource(int imageWidth, int imageHeight, int tileWidth, int tileHeight, int tileOverlap)
		{
			
		}
		//
		// Summary:
		//     Creates a new instance of the System.Windows.Media.MultiScaleTileSource class
		//     with the specified parameters.
		//
		// Parameters:
		//   imageWidth:
		//     The width of the Deep Zoom image.
		//
		//   imageHeight:
		//     The height of the Deep Zoom image.
		//
		//   tileWidth:
		//     The width of the tiles in the Deep Zoom image.
		//
		//   tileHeight:
		//     The height of the tiles in the Deep Zoom image.
		//
		//   tileOverlap:
		//     How much the tiles in the Deep Zoom image overlap.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     One or more arguments are invalid.
        [OpenSilver.NotImplemented]
		public MultiScaleTileSource(long imageWidth, long imageHeight, int tileWidth, int tileHeight, int tileOverlap)
		{
			
		}

		//
		// Summary:
		//     Gets or sets the amount of time to blend a new level of detail when a tile becomes
		//     available.
		//
		// Returns:
		//     The amount of time to blend a new level of detail when a tile becomes available.
        [OpenSilver.NotImplemented]
		protected TimeSpan TileBlendTime { get; set; }

		//
		// Summary:
		//     Gets a collection of the URIs that comprise the Deep Zoom image.
		//
		// Parameters:
		//   tileLevel:
		//     Level of the tile.
		//
		//   tilePositionX:
		//     X-coordinate position of the tile.
		//
		//   tilePositionY:
		//     Y-coordinate position of the tile.
		//
		//   tileImageLayerSources:
		//     Source of the tile image layer, which is a collection of URIs.
        [OpenSilver.NotImplemented]
		protected abstract void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, IList<object> tileImageLayerSources);
		//
		// Summary:
		//     Invalidates specified tile layers.
		//
		// Parameters:
		//   level:
		//     Tile level.
		//
		//   tilePositionX:
		//     X position of the tile.
		//
		//   tilePositionY:
		//     Y position of the tile.
		//
		//   tileLayer:
		//     Layer of the tile.
        [OpenSilver.NotImplemented]
		protected void InvalidateTileLayer(int level, int tilePositionX, int tilePositionY, int tileLayer)
		{
			
		}
	}
}
