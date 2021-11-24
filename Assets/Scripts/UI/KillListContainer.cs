using UnityEngine;

public class KillListContainer : MonoBehaviour
{
    [SerializeField]private KillListLine[] arrLines;
    private bool _firstLineEmpty = true;

    public void ShowNewDeath(int player1,int player2)
    {
        if (_firstLineEmpty)
        {
            arrLines[0].SetLine(player1,player2);
            _firstLineEmpty = false;
        }
        else
        {
            arrLines[1].CopyLine(arrLines[0]);
            arrLines[0].SetLine(player1,player2);
        }
    }
    
}
