using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    private static List<GameObject> EvidenceList;

    public static void Init()
    {
        if (EvidenceList == null)
        {
            EvidenceList = new List<GameObject>();
        }
    }

    public static void AddEvidence(GameObject[] newEvidenceList)
    {
        bool withinList = false;
        if (newEvidenceList == null)
        {
            return;
        }
        foreach (GameObject newEvidence in newEvidenceList)
        {
            foreach (GameObject evidence in EvidenceList)
            {
                if (evidence == newEvidence)
                {
                    withinList = true;
                    break;
                }
            }
            if (!withinList)
            {
                EvidenceList.Add(newEvidence);
            }
        }
        
    }
}
