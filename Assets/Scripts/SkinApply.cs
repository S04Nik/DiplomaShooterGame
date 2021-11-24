using Com.Tereshchuk.Shooter;
using Photon.Pun;
using UnityEngine;

public class SkinApply : MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField] private Transform[] maleCharacterSkins;
    [SerializeField] private Transform[] femaleCharacterSkins;
    [SerializeField] private Transform[] hats;
    [SerializeField] private Transform[] maleHairCuts;
    [SerializeField] private Transform[] femaleHairCuts;
    [SerializeField] private Transform[] beards;
    [SerializeField] private PlayerController playerController;
    
    string _skinName ;
    private int _genderIndx, _hatIndx, _hairCutIndx, _beardIndx;

    public void ApplySettings()
    {
        PlayerCustomization.Instance._settings.Skins.TryGetValue( PhotonNetwork.LocalPlayer.NickName,out _skinName);
        PlayerCustomization.Instance._settings.Genders.TryGetValue( PhotonNetwork.LocalPlayer.NickName,out _genderIndx);
        
        PlayerCustomization.Instance._settings.Hats.TryGetValue( PhotonNetwork.LocalPlayer.NickName,out _hatIndx);
        PlayerCustomization.Instance._settings.HairCuts.TryGetValue( PhotonNetwork.LocalPlayer.NickName,out _hairCutIndx);
        PlayerCustomization.Instance._settings.Beards.TryGetValue( PhotonNetwork.LocalPlayer.NickName,out _beardIndx);

        ActivateProperSkin();
    }
    
    // INDEXS AT ARRAY SHOULD BE SIMILAR 
    // MADE LIKE THIS BECAUSE OF STRING ( i think) consumes more data
    private void ActivateProperSkin()
    {
        playerController.SetGenderVoice(_genderIndx);
        if (_genderIndx == 0)
        {
            foreach (Transform s in maleCharacterSkins)
            {
                if (s.name.Equals(_skinName))
                    s.gameObject.SetActive(true);
            }
            for (int i =0 ; i < maleHairCuts.Length;i++)
            {
                if (i ==_hairCutIndx)
                    maleHairCuts[i].gameObject.SetActive(true);
            }
            for (int i =0 ; i < beards.Length;i++)
            {
                if (i ==_beardIndx)
                    beards[i].gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform s in femaleCharacterSkins)
            {
                if (s.name.Equals(_skinName))
                    s.gameObject.SetActive(true);
            }
            for (int i =0 ; i < femaleHairCuts.Length;i++)
            {
                if (i ==_hairCutIndx)
                    femaleHairCuts[i].gameObject.SetActive(true);
            }
            beards[0].gameObject.SetActive(true); // NO BEARD
        }
        
        for (int i =0 ; i < hats.Length;i++)
        {
            if (i ==_hatIndx)
                hats[i].gameObject.SetActive(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_skinName);
            stream.SendNext(_genderIndx);
            stream.SendNext(_hatIndx);
            stream.SendNext(_hairCutIndx);
            stream.SendNext(_beardIndx);
        }
        else {
            _skinName =(string) stream.ReceiveNext();
            _genderIndx =(int) stream.ReceiveNext();
            _hatIndx =(int) stream.ReceiveNext();
            _hairCutIndx =(int) stream.ReceiveNext();
            _beardIndx =(int) stream.ReceiveNext();
            
            ActivateProperSkin(); 
        }
    }
}
