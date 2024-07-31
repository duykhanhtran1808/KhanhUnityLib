using System.Collections;
using UnityEngine;

    public class BackGroundAnim : MonoBehaviour
    {

        //=================Xử lý trực thăng================
        [SerializeField]
        private GameObject helicopter; //GameObject cái trực thăng
        private Vector3 helicopter_point1 = new Vector2(120, 56); // Các điểm trực thăng bay đến, 
        private Vector3 helicopter_point2 = new Vector2(-115, 3);
        private Vector3 helicopter_point3 = new Vector2(-210, 88);

        private Vector3 helicopter_destination; //điểm đến tiếp theo

        //Góc quay của trực thăng,cần tối ưu lại
        private Quaternion helicopter_rotation_positive = new Quaternion(0, 180, 0, 1);
        private Quaternion helicopter_rotation_negative = new Quaternion(0, 0, 0, 1);

        private int currentPos = 1; //Vị trí hiện tại

        //=================Xử lý trực thăng================
        private Coroutine heli_time = null; //Cách 2:Coroutine
        private float lastTime;  //Cách 1: Time.time

        Quaternion q; 

        void Start()
        {
            helicopter.transform.localPosition = helicopter_point1; //Khởi tạo vị trí đầu tiên
            lastTime = Time.time; //Đặt thời gian
            heli_time = StartCoroutine(MoveHeli(4f));

        }

        // Update is called once per frame
        void Update()
        {
            //=========Cách 1: Time.time==========
            //if(Time.time - lastTime >= 4f)
            //{
            //    lastTime = Time.time;
            //    SetDestinationAndRotation();
            //}
            //=========Cách 1: Time.time==========

            helicopter.transform.localPosition = Vector3.MoveTowards(helicopter.transform.localPosition, helicopter_destination, 1f); //Di chuyển trực thăng

            helicopter.transform.rotation = Quaternion.Slerp(helicopter.transform.rotation, q, Time.deltaTime * 3f); //Cho trực thăng cắm mặt đến đích

        }

        //===========Cách 2: Coroutine===================
        IEnumerator MoveHeli(float delayTime)
        {
            SetDestinationAndRotation();
            yield return new WaitForSeconds(delayTime);
            StopCoroutine(heli_time);
            heli_time = StartCoroutine(MoveHeli(4f));
        }
        //===========Cách 2: Coroutine===================


        //======Di chuyển và xoay trực thăng=========
        private void SetDestinationAndRotation()
        {
            //Thay đổi vị trí dựa trên vị trí trước
            switch (currentPos)
            {
                case 1: currentPos = 2; helicopter_destination = helicopter_point2; break;

                case 2: currentPos = 3; helicopter_destination = helicopter_point3; break;

                case 3: currentPos = 1; helicopter_destination = helicopter_point1; break;
            }
            //Lật hướng trực thăng
            int newX = helicopter_destination.x - helicopter.transform.localPosition.x > 0 ? 1 : -1;
            helicopter.transform.localScale = new Vector3(newX, helicopter.transform.localScale.y, helicopter.transform.localScale.z);

            //Vector hướng đến đích - xoay đầu trực thăng cắm đến đích
            Vector3 vectorToTarget1 = helicopter.transform.position - helicopter_destination;
            float angleTan = Mathf.Atan2(vectorToTarget1.y, vectorToTarget1.x) * Mathf.Rad2Deg;
            //Góc xoay
            float rotateAngle = (helicopter.transform.localScale.x > 0) ? (180 - angleTan) : angleTan;
            q = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        }
    }
