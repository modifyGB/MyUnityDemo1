using GridSystem;
using Place;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class BuildManager : Singleton<BuildManager>
    {
        [Header("资源")]
        public Material canBuildMaterial;
        public Material noBuildMaterial;

        private PlaceObject previewObject = null;
        private PlaceSO buildObject = null;
        public PlaceSO BuildObject
        {
            get { return buildObject; }
            set
            {
                if (buildObject == value)
                    return;
                buildObject = value;
                if (isBuilding)
                {
                    if (value != null)
                        CreatePreview();
                    else
                        DestroyPreview();
                }
            }
        }
        private bool isBuilding = false;
        public bool IsBuilding
        {
            get { return isBuilding; }
            set
            {
                isBuilding = value;
                if (isBuilding)
                    CreatePreview();
                else
                    DestroyPreview();

            }
        }

        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private bool isMouse0 = false;

        public override void Awake()
        {
            base.Awake();

            inputDic.Add(KeyCode.R, false);
            inputDic.Add(KeyCode.Z, false);
            inputDic.Add(KeyCode.V, false);
        }

        private void Update()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                var item = inputDic.ElementAt(i);
                if (!inputDic[item.Key])
                    inputDic[item.Key] = Input.GetKeyDown(item.Key);
            }
            if (!isMouse0)
                isMouse0 = Input.GetMouseButtonDown(0);
        }

        private void FixedUpdate()
        {
            if (inputDic[KeyCode.Z])
            {
                IsBuilding = !IsBuilding;
                inputDic[KeyCode.Z] = false;
            }

            if (previewObject == null)
            {
                isMouse0 = false;
                inputDic[KeyCode.R] = false;
                return;
            }

            if (isMouse0)
            {
                var mousePosition = Utils.MouseToTerrainPosition(MapManager.I.groundLayerName);
                if (UIManager.I.UIState == UIState.Play && Place(mousePosition) == null)
                    print("can't build here");
                else
                {
                    var bag = PlayerManager.I.PlayerBag;
                    bag.Bag.ItemList[bag.UseNum].Count -= 1;
                    bag.Bag.DeleteCheck(bag.UseNum);
                }
                isMouse0 = false;
            }

            if (inputDic[KeyCode.R])
            {
                previewObject.NextDir();
                inputDic[KeyCode.R] = false;
            }
        }

        private void LateUpdate()
        {
            UpdatePreview();
        }

        //创建预置物体
        public void CreatePreview()
        {
            DestroyPreview();
            if (BuildObject == null)
                return;

            previewObject = Instantiate(BuildObject.placeObject, Vector3.zero, Quaternion.identity);
            previewObject.transform.SetParent(MapManager.I.map);

            var colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
                collider.enabled = false;

            Utils.SetMaterial(previewObject.gameObject, canBuildMaterial);
        }
        //销毁预置物体
        public void DestroyPreview()
        {
            if (previewObject != null)
            {
                previewObject.DestroySelf();
                previewObject = null;
            }
        }
        //更新预置物体
        public void UpdatePreview()
        {
            if (!IsBuilding || previewObject == null)
                return;

            var mousePosition = Utils.MouseToTerrainPosition(MapManager.I.groundLayerName);
            var gridPosition = MapManager.I.grid.GetXZ(mousePosition);
            var currentRotation = Quaternion.Euler(0, Utils.GetRotationAngle(previewObject.Dir), 0);
            var rotationOffset = Utils.GetRotationOffset(previewObject.Dir);
            var targetPosition = MapManager.I.grid.GetWorldPosition(gridPosition.x, gridPosition.y)
                               + new Vector3(rotationOffset.x, 0, rotationOffset.y) * MapManager.I.grid.CellSize;
            targetPosition.y += 1f;
            previewObject.gameObject.transform.position =
                Vector3.Lerp(previewObject.gameObject.transform.position, targetPosition, Time.deltaTime * 15f);
            previewObject.gameObject.transform.rotation =
                Quaternion.Lerp(previewObject.gameObject.transform.rotation, currentRotation, Time.deltaTime * 15f);

            if (BuildObject.BuildCheck(gridPosition, previewObject.Dir))
                Utils.SetMaterial(previewObject.gameObject, canBuildMaterial);
            else
                Utils.SetMaterial(previewObject.gameObject, noBuildMaterial);
        }
        //放置物体
        public PlaceObject Place(Vector3 position)
        {
            if (BuildObject == null)
                return null;
            if (position == Vector3.zero)
                return null;
            var origin = MapManager.I.grid.GetXZ(position);
            if (BuildObject.BuildCheck(origin, previewObject.Dir))
            {
                var newObject = BuildObject.Create(origin, previewObject.Dir);
                newObject.transform.SetParent(MapManager.I.map);
                return newObject;
            }
            return null;
        }
        //删除物体
        public void Delete(GridObject gridObject)
        {
            var placedObject = gridObject.PlaceableObject;
            if (placedObject == null)
                return;

            foreach (var gridPos in placedObject.GetGridPositionList())
                MapManager.I.grid.GetGridObject(gridPos.x, gridPos.y).ClearPlaceableObject();
            placedObject.DestroySelf();
        }
    }
}
