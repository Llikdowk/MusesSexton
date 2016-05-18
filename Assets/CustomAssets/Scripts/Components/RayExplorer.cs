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
        private RaycastHit hit;
        private Ray ray;
        private float time_created = 0f;
        private const float startDelay = .25f;
        private const int mask = ~(1<<9 + 1<<8);

        private Vector3i hollowAreaSize;
        private float hollowYOffset;
        private const float maxYOffsetAllowed = .5f;
        private bool restrictionsPassed = false;
        private GameObject impacted;

        private GameObject graveAsset;
        private GameObject dirtAsset;
        private GameObject tombstoneAsset;


        public void Awake() {
            terrainData = terrain.terrainData;
            graveAsset = Resources.Load<GameObject>("Models/GraveAsset");
            dirtAsset = Resources.Load<GameObject>("Models/DirtAsset");
            tombstoneAsset = Resources.Load<GameObject>("Prefabs/TombstonePrefab");
        }

        public void Start () {
            time_created = Time.time;
            clickToCarve = voxelTerrain.GetComponent<ClickToCarveTerrainVolume>();
            hollowAreaSize = new Vector3i(clickToCarve.rangeX, clickToCarve.rangeY, clickToCarve.rangeZ);
        }
        
        public void OnEnable() {
            time_created = Time.time;
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
                impacted = hit.collider.gameObject;
                Debug.DrawRay(Player.Player.getInstance().eyeSight.position, hit.point - this.gameObject.transform.position, Color.red);
                if (impacted.tag == "coffin") {
                    if (GameActions.checkAction(Action.USE, Input.GetKeyDown))
                        coffinAction();
                }
                else if (impacted.name.Contains("OctreeNode") && hit.distance > minDistance) {
                    if (!Player.Player.getInstance().digNewHolesDisabled) {
                        if (checkDiggingRestrictions(hit)) {
                            restrictionsPassed = true;
                            if (GameActions.checkAction(Action.USE, Input.GetKeyDown)) {
                                clickToCarve.doAction();
                                createHollowEntity(hollowAreaSize.x + 1, hollowAreaSize.y + 1, hollowAreaSize.z + 1);
                            }
                        }
                    }
                }
            } else {
                impacted = null;
            }

            showInfoMsg();
        }

        private bool checkDiggingRestrictions(RaycastHit hit) {
            Transform p = Player.Player.getInstance().gameObject.transform;
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log(angle);
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
            GameObject plane = Object.Instantiate(graveAsset); //GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject heap = Object.Instantiate(dirtAsset); //GameObject.CreatePrimitive(PrimitiveType.Sphere);
            heap.tag = "groundHeap";
            plane.tag = "groundGrave";
            plane.transform.position = new Vector3(parent.transform.position.x, hollowYOffset, parent.transform.position.z);
            plane.transform.parent = parent.transform;
            plane.transform.localEulerAngles = new Vector3(0, 90, 0);
            plane.transform.localScale = new Vector3(1.00f, .70f, 1.0f);
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

            heap.transform.parent = parent.transform;
            heap.transform.localScale = new Vector3(1.00f, 0.21f, 1.00f);
            heap.transform.localPosition = new Vector3(-3.37f, -0.05f, 0.31f); //lastOffset: new Vector3(-3.37f, 0.24f, 0.31f); // Vector3.zero + sizeX / 2f * Vector3.right + Vector3.up;
            MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            mr.material = groundGrave;

            GameObject tombstone = Object.Instantiate(tombstoneAsset);
            tombstone.transform.parent = parent.transform;
            tombstone.transform.localEulerAngles = new Vector3(0, -90, 0);
            tombstone.transform.localPosition = new Vector3(0, -2.25f, 3.00f);
            tombstone.AddComponent<TombstoneController>();

            trigger_hollow_behaviours t = triggerThrowCoffin.AddComponent<trigger_hollow_behaviours>();
            t.init(AnimationUtils.createThrowCoffinCurve(), parent.transform, plane, heap, tombstone);

            GameObject playerPosition = new GameObject("PlayerPosition");
            playerPosition.transform.parent = parent.transform;
            playerPosition.transform.localPosition = new Vector3(0.20f, 1.30f, -2.02f);
            Player.Player.getInstance().doMovementDisplacement(playerPosition.transform);

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

        private void showInfoMsg() {
            if (impacted == null) { UIUtils.infoInteractive.text = ""; return; }

            if (impacted.tag == "coffin")
                UIUtils.infoInteractive.text = "drag coffin!";
            else if (restrictionsPassed) {
                UIUtils.infoInteractive.text = "dig terrain!";
                restrictionsPassed = false;
            }
            else {
                UIUtils.infoInteractive.text = "";
            }
                        
        }

    }
}
