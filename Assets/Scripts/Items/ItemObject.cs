using GridSystem;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class ItemObject : MyScript
    {
        public Item item;

        private Rigidbody rb;
        public Rigidbody Rb { get { return rb; } }
        private GameObject countObject;
        private GameObject dureObject;
        private TextMeshProUGUI countText;
        public TextMeshProUGUI CountText { get { return countText; } }
        private GameObject Nowdure;

        private bool isThrow = false;
        public bool IsThrow
        {
            get { return isThrow; }
            set
            {
                if (value)
                {
                    countObject.SetActive(false);
                    dureObject.SetActive(false);
                    transform.localScale = Vector3.one * 0.01f;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
                else
                {
                    if (item.itemSO.isCountable)
                        countObject.SetActive(true);
                    if (item.itemSO.isDurable)
                        dureObject.SetActive(true);
                    transform.localScale = Vector3.one;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX |
                        RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    transform.rotation = Quaternion.identity;
                }
                isThrow = value;
            }
        }
        private bool isPick = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            countObject = Utils.FindChildByName(gameObject, "count");
            dureObject = Utils.FindChildByName(gameObject, "dure");
            countText = countObject.GetComponent<TextMeshProUGUI>();
            Nowdure = Utils.FindChildByName(gameObject, "nowDure");
        }

        public virtual void Update()
        {
            if (isThrow)
                transform.rotation = Camera.main.transform.rotation;
        }
        //拾取
        private void OnCollisionEnter(Collision collisionInfo)
        {
            if (isPick)
                return;
            if (IsThrow && collisionInfo.gameObject.tag == "Player")
            {
                isPick = true;
                var x = PlayerManager.I.PlayerBag.Bag.AddBag(item.ToSerialization());
                if (x != null)
                    x.Throw(PlayerManager.I.ThrowPoint, PlayerManager.Player.transform.forward * 5);
                else
                    SoundManager.I.Pick();
                item.DestroySelf();
            }
        }
        //设置count文本
        public void UpdateCountText()
        {
            if (!item.itemSO.isCountable || countObject == null)
                return;
            if (item.IsUnlimited)
                countText.text = "∞";
            else
            {
                if (item.Count < 10)
                    countText.text = "0" + item.Count.ToString();
                else
                    countText.text = item.Count.ToString();
                if (item.Count == 1 || IsThrow)
                    countObject.SetActive(false);
                else
                    countObject.SetActive(true);
            }
        }
        //设置dure长度
        public void UpdateDureLength()
        {
            if (!item.itemSO.isDurable || dureObject == null)
                return;
            Nowdure.transform.localScale = new Vector3(item.Dure * 1.0f / item.itemSO.MaxDure, 1, 1);
            Nowdure.transform.localPosition = new Vector3((item.itemSO.MaxDure - item.Dure) / item.itemSO.MaxDure * 90 * -0.5f, 0, 0);
        }
        //初始化
        public void Initialization(Item item, bool isThrow)
        {
            this.item = item;

            if (!item.itemSO.isCountable)
                countObject.SetActive(false);
            if (!item.itemSO.isDurable)
                dureObject.SetActive(false);

            if (isThrow)
                IsThrow = isThrow;

            item.ItemChange += UpdateCountText;
            item.ItemChange += UpdateDureLength;
            UpdateCountText();
            UpdateDureLength();
        }
    }
}
