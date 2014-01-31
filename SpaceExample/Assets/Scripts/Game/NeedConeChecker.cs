using UnityEngine;
using System.Collections;

public class NeedConeChecker : MonoBehaviour {

    public ConeChecker checker
    {
        get;
        private set;
    }

    public GameObject coneCheckerPrefab;

	// Use this for initialization
	void Start () {
        GameObject coneCheckerObject = (GameObject)Instantiate(coneCheckerPrefab);
        checker = coneCheckerObject.GetComponent<ConeChecker>();
        checker.objectToCheckFor = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
