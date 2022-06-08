using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BOX : MonoBehaviour 
{
    [SerializeField]
    private StageControl m_stageControl;
    private BlockRoot m_blockRoot;

    [SerializeField] Camera m_camera;

    [SerializeField] float speed;

    private Rigidbody m_rigid;
    bool m_debuffs = false;
    float m_debuffsTime = 3.0f;

    private void Awake()
    {
        m_rigid = this.GetComponent<Rigidbody>();
    }

    public void InitStageControl(StageControl stageControl)
    {
        m_stageControl = stageControl;
        m_blockRoot = m_stageControl.blockRoot;
    }

    private void Update()
    {
        if (m_stageControl.Pause == true) return;

        if (m_debuffs == true)
        {
            m_debuffsTime -= Time.deltaTime;
            if (m_debuffsTime < 0f)
            {
                m_debuffsTime = 3.0f;
                m_debuffs = false;
                this.transform.localScale = new Vector3(2.5f, 1.5f, 1.0f);
                speed = 10f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_stageControl.Pause == true) return;

        float inputX = Input.GetAxis("Horizontal");

        transform.Translate(inputX * speed * Time.deltaTime, 0, 0);

       if(transform.position.x > 4.5f)
            transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
        if (transform.position.x < -4.5f)
            transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z);

    }

    //public void OnMouseDrag()
    //{

    //    Vector3 mousePosition = new Vector3
    //        (Input.mousePosition.x, Input.mousePosition.y , 10);
    //    float x;
    //    float y = transform.position.y;
    //    float z = transform.position.z;
    //    this.transform.position = m_camera.ScreenToWorldPoint(mousePosition);

    //    x = transform.position.x;
    //    if (x < -5f)
    //    this.transform.position = new Vector3(-5f, y, z);
    //    else if(x>5f)
    //    this.transform.position = new Vector3(5f, y, z);
    //    else
    //    this.transform.position = new Vector3(x, y, z);
    //}

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            m_stageControl.EatItem(other.gameObject);
        }
    }

    public void GetDebuffs()
    {
        m_debuffs = true;
        this.transform.localScale = new Vector3(1.2f, 1.0f, 1.0f);
        speed = 6f;
    }
}
