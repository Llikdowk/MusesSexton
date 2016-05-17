using UnityEngine;
using System.Collections;

using Cubiquity;

public class ClickToCarveTerrainVolume : MonoBehaviour
{
    public int rangeX = 2;
    public int rangeY = 4;
    public int rangeZ = 2;
    public int smoothFactor = 2;
    public bool euclidean = false;
    public bool erase = true;
    private TerrainVolume terrainVolume;
	
	// Bit of a hack - we want to detect mouse clicks rather than the mouse simply being down,
	// but we can't use OnMouseDown because the voxel terrain doesn't have a collider (the
	// individual pieces do, but not the parent). So we define a click as the mouse being down
	// but not being down on the previous frame. We'll fix this better in the future...
	private bool isMouseAlreadyDown = false;
	
	// Use this for initialization
	void Start ()
	{
		// We'll store a reference to the colored cubes volume so we can interact with it later.
		terrainVolume = gameObject.GetComponent<TerrainVolume>();
		if(terrainVolume == null)
		{
			Debug.LogError("This 'ClickToCarveTerrainVolume' script should be attached to a game object with a TerrainVolume component");
		}
	}


	public void doAction()
	{
		// Bail out if we're not attached to a terrain.
		if(terrainVolume == null)
		{
			return;
		}
		
		// If the mouse btton is down and it was not down last frame
		// then we consider this a click, and do our destruction.
		//if(Input.GetMouseButton(0))
		//{
			if(!isMouseAlreadyDown)
			{
				// Build a ray based on the current mouse position
				Vector2 mousePos = Input.mousePosition;
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));				
				
				// Perform the raycasting.
				PickSurfaceResult pickResult;
				bool hit = Picking.PickSurface(terrainVolume, ray, 1000.0f, out pickResult);
				
				// If we hit a solid voxel then create an explosion at this point.
				if(hit) {					
					DestroyVoxels((int)pickResult.volumeSpacePos.x, (int)pickResult.volumeSpacePos.y, (int)pickResult.volumeSpacePos.z);
				}
				
				// Set this flag so the click won't be processed again next frame.
				//isMouseAlreadyDown = true;
			}
		//}
		else
		{
			// Clear the flag while we wait for a click.
			isMouseAlreadyDown = false;
		}
	}
	
	void DestroyVoxels(int xPos, int yPos, int zPos)
	{
        // Initialise outside the loop, but we'll use it later.
        int aux = Mathf.Max(rangeX, rangeY, rangeZ);
        int rangeSquared = aux*aux;
        MaterialSet emptyMaterialSet = new MaterialSet();
        MaterialSet fillMaterialSet = new MaterialSet();
        fillMaterialSet.weights[0] = 255;
        fillMaterialSet.weights[1] = 255;
        fillMaterialSet.weights[2] = 255;

        // Iterage over every voxel in a cubic region defined by the received position (the center) and
        // the range. It is quite possible that this will be hundreds or even thousands of voxels.
        for (int z = zPos - rangeZ; z < zPos + rangeZ; z++) 
		{
			for(int y = yPos - rangeY; y < yPos + rangeY; y++)
			{
				for(int x = xPos - rangeX; x < xPos + rangeX; x++)
				{			
					// Compute the distance from the current voxel to the center of our explosion.
					int xDistance = x - xPos;
					int yDistance = y - yPos;
					int zDistance = z - zPos;

                    // Working with squared distances avoids costly square root operations.
                    int distSquared = 0;
                    if (euclidean) {
                        distSquared = sqrEuclideanDistance(xDistance, yDistance, zDistance);
                    } else {
                        distSquared = manhattanDistance(xDistance, yDistance, zDistance);
                    }
					
					// We're iterating over a cubic region, but we want our explosion to be spherical. Therefore 
					// we only further consider voxels which are within the required range of our explosion center. 
					// The corners of the cubic region we are iterating over will fail the following test.
					if(distSquared < rangeSquared) {
                        if (erase) {
                            terrainVolume.data.SetVoxel(x, y, z, emptyMaterialSet);
                        } else {
                            terrainVolume.data.SetVoxel(x, y, z, fillMaterialSet);
                        }
					}
				}
			}
		}
		
		TerrainVolumeEditor.BlurTerrainVolume(terrainVolume,
            new Region(
                xPos - rangeX + smoothFactor,
                yPos - rangeY + smoothFactor,
                zPos - rangeZ + smoothFactor,
                xPos + rangeX + smoothFactor,
                yPos + rangeY + smoothFactor,
                zPos + rangeZ + smoothFactor)
            );
		//TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
		//TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
	}

    private int sqrEuclideanDistance(int x, int y, int z) {
        return x*x + y*y + z*z;
    }

    private int manhattanDistance(int x, int y, int z) {
        return x + y + z;
    }




    /*
    public void doActionRelativeSpace(Transform local) {
        if (terrainVolume == null) {
            return;
        }
        if (!isMouseAlreadyDown) {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
            PickSurfaceResult pickResult;
            bool hit = Picking.PickSurface(terrainVolume, ray, 1000.0f, out pickResult);
            if (hit) {
                DestroyRelativeVoxels((int)pickResult.volumeSpacePos.x, (int)pickResult.volumeSpacePos.y, (int)pickResult.volumeSpacePos.z, local);
            }
        }
        else {
            isMouseAlreadyDown = false;
        }
    }

    private void DestroyRelativeVoxels(int xPos, int yPos, int zPos, Transform local) {
        // Initialise outside the loop, but we'll use it later.
        int aux = Mathf.Max(rangeX, rangeY, rangeZ);
        int rangeSquared = aux * aux;
        MaterialSet emptyMaterialSet = new MaterialSet();
        MaterialSet fillMaterialSet = new MaterialSet();
        fillMaterialSet.weights[0] = 255;
        fillMaterialSet.weights[1] = 255;
        fillMaterialSet.weights[2] = 255;

        Vector3 positions = transform.TransformPoint(new Vector3(xPos, yPos, zPos));
        xPos = (int)positions.x;
        yPos = (int)positions.y;
        zPos = (int)positions.z;

        // Iterage over every voxel in a cubic region defined by the received position (the center) and
        // the range. It is quite possible that this will be hundreds or even thousands of voxels.
        for (int z = zPos - rangeZ; z < zPos + rangeZ; z++) {
            for (int y = yPos - rangeY; y < yPos + rangeY; y++) {
                for (int x = xPos - rangeX; x < xPos + rangeX; x++) {
                    // Compute the distance from the current voxel to the center of our explosion.
                    Vector3 v = transform.TransformPoint(new Vector3(x, y, z));
                    x = (int)v.x;
                    y = (int)v.y;
                    z = (int)v.z;
                    int xDistance = x - xPos;
                    int yDistance = y - yPos;
                    int zDistance = z - zPos;

                    // Working with squared distances avoids costly square root operations.
                    int distSquared = 0;
                    if (euclidean) {
                        distSquared = sqrEuclideanDistance(xDistance, yDistance, zDistance);
                    }
                    else {
                        distSquared = manhattanDistance(xDistance, yDistance, zDistance);
                    }

                    // We're iterating over a cubic region, but we want our explosion to be spherical. Therefore 
                    // we only further consider voxels which are within the required range of our explosion center. 
                    // The corners of the cubic region we are iterating over will fail the following test.
                    if (distSquared < rangeSquared) {
                        if (erase) {
                            terrainVolume.data.SetVoxel(x, y, z, emptyMaterialSet);
                        }
                        else {
                            terrainVolume.data.SetVoxel(x, y, z, fillMaterialSet);
                        }
                    }
                }
            }
        }

        TerrainVolumeEditor.BlurTerrainVolume(terrainVolume,
            new Region(
                xPos - rangeX + smoothFactor,
                yPos - rangeY + smoothFactor,
                zPos - rangeZ + smoothFactor,
                xPos + rangeX + smoothFactor,
                yPos + rangeY + smoothFactor,
                zPos + rangeZ + smoothFactor)
            );
        //TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
        //TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
    }
    */
}
