using System.Collections.Generic;
using System.Linq;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;

namespace Assets.CustomAssets.Scripts {
    public class DigBehaviour : MonoBehaviour {
        public int maxDistance = 15;
        public int minDistance = 2;
        public Material groundGrave;
        public Material groundHeap;
        public Terrain terrain;
        private TerrainData terrainData;
        private float[,] heightMap;
        private RaycastHit hit;
        private Ray ray;
        private float time_created = 0f;
        private const float startDelay = .25f;

        public void Awake() {
            //terrain = Terrain.activeTerrain;
            terrainData = terrain.terrainData;
            //Debug.Log("HEIGHTMAP DATA: " + "size[0]: " + heightMap.GetLength(0) + " size[1]: " + heightMap.GetLength(1));
        }

        public void Start () {
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
            time_created = Time.time;
        }
        
        public void OnEnable() {
            time_created = Time.time;
        }

        public void Update () {
            if (Time.time - time_created < startDelay) { return; }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, maxDistance)) {
                GameObject impacted = hit.collider.gameObject;
                Debug.DrawRay(Player.Player.getInstance().eyeSight.position, hit.point - this.gameObject.transform.position, Color.red);
                if (GameActions.checkAction(Action.USE, Input.GetKey)) {
                    if (impacted.tag == "terrain") {
                        if (hit.distance < minDistance) return;
                        Debug.DrawRay(Player.Player.getInstance().eyeSight.position, hit.point - this.gameObject.transform.position, Color.magenta);
                        //Debug.Log(hit.point);
                        Vector3 p = terrain.gameObject.transform.InverseTransformPoint(hit.point);

                        Vector2 vertex = new Vector2((p.x / terrainData.size.x) * terrainData.heightmapResolution, (p.z / terrainData.size.z) * terrainData.heightmapResolution);
                        //Debug.Log("Res = " + terrainData.heightmapResolution + " width: " + terrainData.size.x + ", " + terrainData.size.z + " pos: " + (int)(p.x) + " " + (int)p.z + " vertex: " + (int)vertex.x + " " + (int) vertex.y);

                        int area_w = 4;
                        int area_h = 4;
                        float[,] h = new float[area_w, area_h];
                        for (int i = 0; i < area_w; ++i) {
                            for (int j = 0; j < area_h; ++j) {
                                h[j, i] = 0.2f;//heightMap[(int)(vertex.y)+i, (int)(vertex.x)+j] - terrainData.heightmapResolution;
                                               //heightMap[(int)(vertex.y) + i, (int)(vertex.x) + j] = h[j, i];
                            }
                        }
                        terrainData.SetHeights((int)(vertex.x) - area_w / 2, (int)(vertex.y) - area_h / 2, h);
                        GameObject parent = new GameObject("Grave");
                        parent.transform.position = hit.point;
                        //BoxCollider diggingForbidden = parent.AddComponent<BoxCollider>();
                        //diggingForbidden.size = new Vector3(2*area_w, 4, 2 * area_h);
                        //diggingForbidden.isTrigger = true;

                        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        GameObject heap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        heap.tag = "groundHeap";
                        plane.tag = "groundGrave";
                        plane.transform.parent = parent.transform;
                        plane.transform.localPosition = -Vector3.up * .25f;
                        heap.transform.parent = parent.transform;
                        heap.transform.localPosition = Vector3.zero + area_w/2f*Vector3.right + Vector3.up;
                        MeshRenderer mr = plane.GetComponent<MeshRenderer>();
                        mr.material = groundGrave;
                        //mr = heap.GetComponent<MeshRenderer>();
                        //mr.material = groundHeap;
                    }
                    else if (impacted.tag == "groundGrave") {
                        impacted.transform.position -= Vector3.up*.25f;
                        impacted.transform.parent.GetChild(1).transform.localScale += Vector3.one * .25f;
                    }
                    else if (impacted.tag == "groundHeap") {
                        impacted.transform.localScale -= Vector3.one * .25f;
                        impacted.transform.parent.GetChild(0).transform.localPosition += Vector3.up * .25f;
                    } else if (impacted.tag == "coffin") {
                        Player.Player.getInstance().behaviour = new CoffinDragBehaviour(gameObject, impacted);
                    }
                    UIUtils.infoInteractive.text = "";
                }
                else {
                    if (impacted.tag == "terrain") {
                        UIUtils.infoInteractive.text = "dig terrain!";
                    }
                    else if (impacted.tag == "groundGrave") {
                        UIUtils.infoInteractive.text = "dig plane!";
                    }
                    else if (impacted.tag == "groundHeap") {
                        UIUtils.infoInteractive.text = "undig!";
                    }
                    else if (impacted.tag == "coffin") {
                        UIUtils.infoInteractive.text = "drag!";
                    } else {
                        UIUtils.infoInteractive.text = "";
                    }
                }
            } else {
                UIUtils.infoInteractive.text = "";
            }
        }
    }
}
