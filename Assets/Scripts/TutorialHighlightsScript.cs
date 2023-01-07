using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHighlightsScript : MonoBehaviour
{
    public GameObject prefabTutorialHighlight;

    public BoardManagerScript boardManagerScript;

    Vector2Int[] lastHighlights;
    List<GameObject> highlightObjects;

    // Start is called before the first frame update
    void Start()
    {
        highlightObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boardManagerScript.state.progression.highlightCoors != lastHighlights) {
            lastHighlights = boardManagerScript.state.progression.highlightCoors;
            foreach (GameObject go in highlightObjects) {
                Destroy(go);
            }
            highlightObjects.Clear();
            if (lastHighlights != null) {
                foreach (Vector2Int coor in lastHighlights) {
                    GameObject highlight = Instantiate(prefabTutorialHighlight);
                    highlight.transform.localPosition = new Vector3(coor.x, coor.y, highlight.transform.localPosition.z);
                    highlightObjects.Add(highlight);
                }
            }
        }
    }
}
