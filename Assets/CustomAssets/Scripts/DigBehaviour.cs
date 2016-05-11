using UnityEngine;

namespace Assets.CustomAssets.Scripts {
    public class DigBehaviour : MonoBehaviour {
        public Terrain terrain;
        private TerrainData terrainData;

        public void Awake() {
            terrainData = terrain.terrainData;
        }

        public void Start () {
	
        }
	
        public void Update () {
	
        }
    }
}
