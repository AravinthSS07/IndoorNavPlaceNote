using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class CustomShapeManager : MonoBehaviour
{
    public NavController navController;
    public List<GameObject> ShapePrefabs = new List<GameObject>();
    public List<ShapeInfo> shapeInfoList = new List<ShapeInfo>();
    public List<GameObject> shapeObjList = new List<GameObject>();

    private bool shapesLoaded = false;

    public void AddShape(Vector3 shapePosition, Quaternion shapeRotation, bool isDestination)
    {
        int typeIndex = isDestination ? 1 : 0;
        ShapeInfo shapeInfo = new ShapeInfo
        {
            px = shapePosition.x,
            py = shapePosition.y,
            pz = shapePosition.z,
            qx = shapeRotation.x,
            qy = shapeRotation.y,
            qz = shapeRotation.z,
            qw = shapeRotation.w,
            shapeType = typeIndex.GetHashCode()
        };
        shapeInfoList.Add(shapeInfo);
        GameObject shape = ShapeFromInfo(shapeInfo);
        shapeObjList.Add(shape);
    }

    public void AddDestinationShape()
    {
        ShapeInfo lastInfo = shapeInfoList[shapeInfoList.Count - 1];
        lastInfo.shapeType = 1.GetHashCode();
        GameObject shape = ShapeFromInfo(lastInfo);
        shape.GetComponent<DiamondBehavior>().Activate(true);
        Destroy(shapeObjList[shapeObjList.Count - 1]);
        shapeObjList.Add(shape);
    }

    public GameObject ShapeFromInfo(ShapeInfo info)
    {
        GameObject shape;
        Vector3 position = new Vector3(info.px, info.py, info.pz);
        if (SceneManager.GetActiveScene().name == "ReadMap" && info.shapeType == 0)
        {
            shape = Instantiate(ShapePrefabs[2]);
        }
        else
        {
            shape = Instantiate(ShapePrefabs[info.shapeType]);
        }
        if (shape.GetComponent<Node>() != null)
        {
            shape.GetComponent<Node>().pos = position;
            Debug.Log(position);
        }
        shape.tag = "waypoint";
        shape.transform.position = position;
        shape.transform.rotation = new Quaternion(info.qx, info.qy, info.qz, info.qw);
        shape.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        return shape;
    }

    public void ClearShapes()
    {
        Debug.Log("CLEARING SHAPES!!!!!!!");
        foreach (var obj in shapeObjList)
        {
            Destroy(obj);
        }
        shapeObjList.Clear();
        shapeInfoList.Clear();
    }

    public JObject Shapes2JSON()
    {
        ShapeList shapeList = new ShapeList();
        shapeList.shapes = shapeInfoList.ToArray();
        return JObject.FromObject(shapeList);
    }

    public void LoadShapesJSON(JToken mapMetadata)
    {
        if (!shapesLoaded)
        {
            shapesLoaded = true;
            Debug.Log("LOADING SHAPES>>>");
            if (mapMetadata is JObject && mapMetadata["shapeList"] is JObject)
            {
                ShapeList shapeList = mapMetadata["shapeList"].ToObject<ShapeList>();
                if (shapeList.shapes == null)
                {
                    Debug.Log("no shapes dropped");
                    return;
                }

                foreach (var shapeInfo in shapeList.shapes)
                {
                    shapeInfoList.Add(shapeInfo);
                    GameObject shape = ShapeFromInfo(shapeInfo);
                    shapeObjList.Add(shape);
                }

                navController?.InitializeNavigation();
            }
        }
    }
}