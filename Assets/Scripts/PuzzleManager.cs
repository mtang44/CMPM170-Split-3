using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int requiredCount = 3;
    private int activatedCount = 0;

    public Final finalObjectController;

    public void ActivateOneObject()
    {
        activatedCount++;

        if (activatedCount >= requiredCount)
        {
            if (finalObjectController != null)
            {
                finalObjectController.ActivateFinalObject();
            }
        }
    }
}
