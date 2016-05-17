using System.Collections.Generic;
using System.Linq;
using Assets.CustomAssets.Scripts.Anmation;
using Assets.CustomAssets.Scripts.CustomInput;
using Assets.CustomAssets.Scripts.Player.Behaviour;
using UnityEngine;
using Cubiquity;

namespace Assets.CustomAssets.Scripts {
    public class RayExplorer : MonoBehaviour {
        private float maxDistance = 5f;
        private float minDistance = 2.5f;
        public Material groundGrave;
        public Material groundHeap;
        public Terrain terrain;

        public GameObject voxelTerrain;
        private ClickToCarveTerrainVolume clickToCarve;

        private TerrainData terrainData;
        private float[,] heightMap;
        private RaycastHit hit;
        private Ray ray;
        private float time_created = 0f;
        private const float startDelay = .25f;
        private const int mask = ~(1<<9 + 1<<8);

        private Vector3i hollowAreaSize;
        private float hollowYOffset;
        private const float maxYOffsetAllowed = .5f;

        public void Awake() {
            terrainData = terrain.terrainData;
        }

        public void Start () {
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
            time_created = Time.time;

            clickToCarve = voxelTerrain.GetComponent<ClickToCarveTerrainVolume>();
            hollowAreaSize = new Vector3i(clickToCarve.rangeX, clickToCarve.rangeY, clickToCarve.rangeZ);
        }
        
        public void OnEnable() {
            time_created = Time.time;
        }

        private GameObject impacted;

        private void terrainAction() {
            //if (hit.distance < minDistance) return;
            Vector3 p = terrain.gameObject.transform.InverseTransformPoint(hit.point);
            Vector2 vertex = new Vector2((p.x / terrainData.size.x) * terrainData.heightmapResolution, (p.z / terrainData.size.z) * terrainData.heightmapResolution);
            //Debug.Log("Res = " + terrainData.heightmapResolution + " width: " + terrainData.size.x + ", " + terrainData.size.z + " pos: " + (int)(p.x) + " " + (int)p.z + " vertex: " + (int)vertex.x + " " + (int) vertex.y);
            const int area_w = 4;
            const int area_h = 4;
            float[,] h = new float[area_w, area_h];
            for (int i = 0; i < area_w; ++i) {
                for (int j = 0; j < area_h; ++j) {
                    h[j, i] = 0.2f;
                }
            }
            terrainData.SetHeights((int)(vertex.x) - area_w / 2, (int)(vertex.y) - area_h / 2, h);
            createHollowEntity(area_w+1, 2, area_h + 1);

        }

        private void groundGraveAction() {
            Player.Player.getInstance().behaviour = new DigBehaviour(gameObject, impacted);
        }

        private void coffinAction() {
            Player.Player.getInstance().behaviour = new CoffinDragBehaviour(gameObject, impacted);
        }

        public void Update () {
            if (Time.time - time_created < startDelay) return;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //PickSurfaceResult pickResult;
            //bool hashit = Picking.PickSurface(voxelTerrain.GetComponent<TerrainVolume>(), ray, maxDistance, out pickResult);
            
            if (Physics.Raycast(ray, out hit, maxDistance, mask)) {
                //Debug.Log("has collided with something! named " + hit.collider.gameObject.name + " tag: " + hit.collider.gameObject.tag + " normal: " + hit.normal + " distance: " + hit.distance + " PARENT: " + hit.collider.gameObject.transform.parent.name);
                impacted = hit.collider.gameObject;
                Debug.DrawRay(Player.Player.getInstance().eyeSight.position, hit.point - this.gameObject.transform.position, Color.red);
                showInfoMsg(impacted);
                if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                    if (impacted.tag == "coffin")
                        coffinAction();
                    else if (impacted.name.Contains("OctreeNode") && hit.distance > minDistance) {
                        if (!Player.Player.getInstance().digNewHolesDisabled) {
                            if (checkDiggingRestrictions(hit)) {
                                clickToCarve.doAction();
                                createHollowEntity(hollowAreaSize.x + 1, hollowAreaSize.y + 1, hollowAreaSize.z + 1);
                            }
                        }
                    }
                }
            }
            else {
                UIUtils.infoInteractive.text = "";
            }
            
        }

        private bool checkDiggingRestrictions(RaycastHit hit) {
            Transform p = Player.Player.getInstance().gameObject.transform;
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            Debug.Log(angle);
            if (angle > 25f) {
                return false;
            }

            return calcMinHollowHeight(hit, out hollowYOffset);
        }

        private void createHollowEntity(int sizeX, int sizeY, int sizeZ) {
            GameObject parent = new GameObject("Grave");
            BoxCollider bc = parent.AddComponent<BoxCollider>();
            Vector3 v = new Vector3(sizeX*1.5f, sizeY*1.5f, sizeZ*1.5f);
            bc.size = v;
            bc.gameObject.layer = 9;
            bc.enabled = false;

            parent.transform.position = hit.point - new Vector3(sizeX/2f, 0, sizeZ/2f);
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject heap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            heap.tag = "groundHeap";
            plane.tag = "groundGrave";
            plane.transform.position = new Vector3(parent.transform.position.x, hollowYOffset, parent.transform.position.z);
            plane.transform.parent = parent.transform;
            plane.transform.localScale = new Vector3(0.4f, 1.00f, 0.7f);
            //plane.transform.localPosition = Vector3.zero + new Vector3(0, hollowYOffset, 0);


            GameObject triggerDisableDigging = new GameObject("Disable digging area");
            SphereCollider sc = triggerDisableDigging.AddComponent<SphereCollider>();
            sc.radius = Mathf.Max(sizeX, sizeZ) * 2f;
            sc.isTrigger = true;
            triggerDisableDigging.transform.parent = parent.transform;
            triggerDisableDigging.transform.localPosition = Vector3.zero;
            triggerDisableDigging.AddComponent<trigger_disable_digging>();

            GameObject triggerThrowCoffin = new GameObject("Trigger throw coffin");
            bc = triggerThrowCoffin.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = v * 2.0f;
            triggerThrowCoffin.transform.parent = parent.transform;
            triggerThrowCoffin.transform.localPosition = Vector3.zero;
            trigger_hollow_behaviours t = triggerThrowCoffin.AddComponent<trigger_hollow_behaviours>();
            t.curve = AnimationUtils.createThrowCoffinCurve();
            t.node1 = parent.transform;
            t.groundFloor = plane;

            heap.transform.parent = parent.transform;
            heap.transform.localPosition = Vector3.zero + sizeX / 2f * Vector3.right + Vector3.up;
            MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            mr.material = groundGrave;

        }

        private bool calcMinHollowHeight(RaycastHit hit, out float yOffset) {
            Vector3 n = hit.normal;
            Vector3 p = hit.point;
            Vector3 v = p + n;
            float minY = float.MaxValue;
            float maxY = float.MinValue;


            RaycastHit offsetHit;
            Ray offsetRay;

            Vector3 upRight = new Vector3(hollowAreaSize.x, 0, hollowAreaSize.z);
            Vector3 upLeft = new Vector3(-hollowAreaSize.x, 0, hollowAreaSize.z);
            Vector3 downRight = new Vector3(hollowAreaSize.x, 0, -hollowAreaSize.z);
            Vector3 downLeft = new Vector3(-hollowAreaSize.x, 0, -hollowAreaSize.z);

            Vector3 up = new Vector3(0, 0, hollowAreaSize.z);
            Vector3 left = new Vector3(-hollowAreaSize.x, 0, 0);
            Vector3 right = new Vector3(hollowAreaSize.x, 0, 0);
            Vector3 down = new Vector3(0, 0, -hollowAreaSize.z);

            Vector3[] allVectors = new Vector3[8];
            allVectors[0] = v + upRight;
            allVectors[1] = v + upLeft;
            allVectors[2] = v + downRight;
            allVectors[3] = v + downLeft;

            allVectors[4] = v + 2f*up;
            allVectors[5] = v + 2f*left;
            allVectors[6] = v + 2f*right;
            allVectors[7] = v + 2f*down;

            foreach (Vector3 t in allVectors) {
                offsetRay = new Ray(t, Vector3.down);
                Debug.DrawRay(t, Vector3.down, Color.magenta);
                if (Physics.Raycast(offsetRay, out offsetHit, 1000.0f)) {
                    if (offsetHit.point.y < minY) {
                        minY = offsetHit.point.y;
                    }
                    if (offsetHit.point.y > maxY) {
                        maxY = offsetHit.point.y;
                    }

                    if (maxY - minY > maxYOffsetAllowed) {
                        yOffset = 0.0f;
                        return false;
                    }
                    //Debug.Log("maxY set: " + maxY + " minY set: " + minY);
                } else {
                    yOffset = 0.0f;
                    return false;
                }
            }
            yOffset = minY - .15f;
            return true;
        }

        private void showInfoMsg(GameObject impacted) {

            if (impacted.tag == "terrain" && hit.distance > minDistance) {
                UIUtils.infoInteractive.text = "TERRAIN SPOTTED!";
                Debug.Log("terrain!");
            }
            else if (impacted.tag == "groundGrave")
                UIUtils.infoInteractive.text = "dig plane!";
            else if (impacted.tag == "groundHeap")
                UIUtils.infoInteractive.text = "undig!";
            else if (impacted.tag == "coffin")
                UIUtils.infoInteractive.text = "drag coffin!";
            else if (impacted.tag == "doNothing") {
                Debug.Log("DO NOGHING");
            }
            
            else if (impacted.name.Contains("OctreeNode") && hit.distance > minDistance && hit.distance > minDistance) {
                if (/*hashit &&*/ !Player.Player.getInstance().digNewHolesDisabled) {
                    if (checkDiggingRestrictions(hit)) {
                        UIUtils.infoInteractive.text = "dig terrain!";
                    }
                }
            }
            else {
                UIUtils.infoInteractive.text = "";
            }
                        
        }

    }
}
