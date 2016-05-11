using System.Collections.Generic;
using System.Linq;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets.CustomAssets.Scripts {
    public class DigBehaviour : MonoBehaviour {
        public int maxDistance = 10;
        public Terrain terrain;
        private TerrainData terrainData;
        private float[,] heightMap;

        public void Awake() {
            terrainData = terrain.terrainData;
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        }

        public void Start () {
	
        }
        private RaycastHit hit;
        private Ray ray;

        public void Update () {

            if (GameActions.checkAction(Action.USE, Input.GetKey)) {

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, maxDistance)) {
                    Debug.DrawRay(this.gameObject.transform.position, hit.point - this.gameObject.transform.position, Color.magenta);
                    //Debug.Log(hit.point);
                    Vector3 p = terrain.gameObject.transform.InverseTransformPoint(hit.point);
                    Debug.Log((int)(p.x) + " " + (int)p.z);
                    hit.triangleIndex // TODO! CHECK THIS property!
                    heightMap[(int)(p.x), (int)(p.z)] -= 1;
                    terrainData.SetHeights(0, 0, heightMap);
                    /*
                    MeshRenderer mr = hit.transform.gameObject.GetComponent<MeshRenderer>();
                    if (mr != null) {
                        Material m = mr.material;
                        if (m != null && m.shader.name == "paintableSurface") {
                            painter.drawDecal(decalShape, hit);
                        }
                    }
                    */
                }
            }
        }
    }
}
