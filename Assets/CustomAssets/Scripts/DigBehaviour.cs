using System.Collections.Generic;
using System.Linq;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts {
    public class DigBehaviour : MonoBehaviour {
        public int maxDistance = 10;
        public GameObject debugObject;
        public Terrain terrain;
        private TerrainData terrainData;
        private float[,] heightMap;
        private RaycastHit hit;
        private Ray ray;

        public void Awake() {
            //Terrain original = terrain;
            //terrain = Instantiate(terrain);
            //original.gameObject.SetActive(false);
            terrainData = terrain.terrainData;
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
            Debug.Log("HEIGHTMAP DATA: " + "size[0]: " + heightMap.GetLength(0) + " size[1]: " + heightMap.GetLength(1));
        }

        public void Start () { 
        }
        

        public void Update () {

            if (GameActions.checkAction(Action.USE, Input.GetKey)) {

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, maxDistance)) {
                    Debug.DrawRay(this.gameObject.transform.position, hit.point - this.gameObject.transform.position, Color.magenta);
                    //Debug.Log(hit.point);
                    Vector3 p = terrain.gameObject.transform.InverseTransformPoint(hit.point);
                    debugObject.transform.position = p;
                    
                    Vector2 vertex = new Vector2(p.x / terrainData.size.x * terrainData.detailHeight, p.z / terrainData.size.z * terrainData.heightmapResolution);
                    //Debug.Log("Res = " + terrainData.heightmapResolution + " width: " + terrainData.size.x + ", " + terrainData.size.z + " pos: " + (int)(p.x) + " " + (int)p.z + " vertex: " + (int)vertex.x + " " + (int) vertex.y);
                    

                    
                    int area_w = 5;
                    int area_h = 5;
                    float[,] h = new float[area_w, area_h];
                    for (int i = 0; i < area_w; ++i) {
                        for (int j = 0; j < area_h; ++j) {
                            h[j, i] = heightMap[(int)(vertex.y)+i, (int)(vertex.x)+j] - 1f/terrainData.heightmapResolution * Time.deltaTime;
                            heightMap[(int)(vertex.y) + i, (int)(vertex.x) + j] = h[j, i];
                        }
                    }
                    terrainData.SetHeights((int)(vertex.x), (int)(vertex.y), h);
                    
                    
                }
            }
        }
    }
}
