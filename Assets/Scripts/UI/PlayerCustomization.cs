using Com.Tereshchuk.Shooter;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    public static PlayerCustomization Instance;
    [HideInInspector]public PlayerSettings _settings;
    [SerializeField] private Transform[] _maleCharacterSkins;
    [SerializeField] private Transform[] _femaleCharacterSkins;
    [SerializeField] private TMP_Dropdown _dropdownGenders;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private Transform[] _hats;
    [SerializeField] private Transform[] _hairCutsMale;
    [SerializeField] private Transform[] _hairCutsFemale;
    [SerializeField] private Transform[] _beards;
    [SerializeField] private GameObject _beardsMenuString;


    private bool beardChosen;
    private int indxSkin;
    private int indxHats;
    private int indxHairCuts;
    private int indxBeards;
    private int indxGender;
    private Transform[] _tmprSkinAccesor;
    private Transform[] _tmprHairAccesor;

    private void Start()
    {
        Instance = this;
        _settings = ScriptableObject.CreateInstance<PlayerSettings>();

        DontDestroyOnLoad(_settings);
        _dropdownGenders.onValueChanged.AddListener(delegate
        {
            OnGenderChange(_dropdownGenders);
        });
        _tmprSkinAccesor = _maleCharacterSkins;
        _tmprHairAccesor = _hairCutsMale;
    }

    public void SetDefaultSkin()
    {
        _settings.Skins.Add(PhotonNetwork.LocalPlayer.NickName,"Character_MilitaryMale_01");
        _settings.Genders.Add(PhotonNetwork.LocalPlayer.NickName,0);
    }

    public void OnGenderChange(TMP_Dropdown dropdown)
    {
        indxGender = dropdown.value;
        if (indxGender == 0)
        {
            _tmprSkinAccesor = _maleCharacterSkins;
            _tmprHairAccesor = _hairCutsMale;
            // beards
            _beardsMenuString.SetActive(true);
            if(beardChosen)
                _beards[indxBeards].gameObject.SetActive(true);
            // skins
            _femaleCharacterSkins[indxSkin].gameObject.SetActive(false);
            _maleCharacterSkins[indxSkin].gameObject.SetActive(true);
            // hair cuts 
            _hairCutsFemale[indxHairCuts].gameObject.SetActive(false);
            _hairCutsMale[indxHairCuts].gameObject.SetActive(true);
        }
        else
        {
            _tmprSkinAccesor = _femaleCharacterSkins;
            _tmprHairAccesor = _hairCutsFemale;
            // beards
            _beardsMenuString.SetActive(false);
            if(beardChosen)
                _beards[indxBeards].gameObject.SetActive(false);
            // skins
            _maleCharacterSkins[indxSkin].gameObject.SetActive(false);
            _femaleCharacterSkins[indxSkin].gameObject.SetActive(true);
            // hair cuts 
            _hairCutsFemale[indxHairCuts].gameObject.SetActive(true);
            _hairCutsMale[indxHairCuts].gameObject.SetActive(false);
        }
    }

    public void MoveToNext(int direction)
    {
        if (direction > 0)
        {
            if (_tmprSkinAccesor.Length-1 <= indxSkin)
            {
                indxSkin = 0;
            }else
                indxSkin++;
        }
        else
        {
            if (indxSkin == 0)
            {
                indxSkin = _tmprSkinAccesor.Length-1;
            }else
                indxSkin--;
        }

        if (indxSkin > 0 && direction>0)
        {
            _tmprSkinAccesor[indxSkin-1].gameObject.SetActive(false);
        }
        else if (direction>0)
        {
            _tmprSkinAccesor[_tmprSkinAccesor.Length-1].gameObject.SetActive(false);
        }

        if (direction < 0 && indxSkin <_tmprSkinAccesor.Length-1)
        {
            _tmprSkinAccesor[indxSkin+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _tmprSkinAccesor[0].gameObject.SetActive(false);
        }
        
        _tmprSkinAccesor[indxSkin].gameObject.SetActive(true);

    }
    public void MoveToNextHat(int direction)
    {
        if (direction > 0)
        {
            if (_hats.Length - 1 <= indxHats)
            {
                indxHats = 0;
            }
            else
                indxHats++;
        }
        else
        {
            if (indxHats == 0)
            {
                indxHats = _hats.Length-1;
            }else
                indxHats--;
        }

        if (indxHats > 0 && direction >0)
        {
            _hats[indxHats-1].gameObject.SetActive(false);
        }
        else if (direction >0)
        {
            _hats[_hats.Length-1].gameObject.SetActive(false);
        }
        
        if (direction < 0 && indxHats <_hats.Length-1)
        {
            _hats[indxHats+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _hats[0].gameObject.SetActive(false);
        }
        
        _hats[indxHats].gameObject.SetActive(true);
    }
    public void MoveToNextBeard(int direction)
    {
        if (beardChosen == false) beardChosen = true;
        if (direction > 0)
        {
            if (_beards.Length - 1 <= indxBeards)
            {
                indxBeards = 0;
            }
            else
                indxBeards++;
        }
        else
        {
            if (indxBeards == 0)
            {
                indxBeards = _beards.Length-1;
            }else
                indxBeards--;
        }

        if (indxBeards > 0 && direction > 0)
        {
            _beards[indxBeards-1].gameObject.SetActive(false);
        }
        else if (direction > 0)
        {
            _beards[_beards.Length-1].gameObject.SetActive(false);
        }
        
        if (direction < 0 && indxBeards <_beards.Length-1)
        {
            _beards[indxBeards+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _beards[0].gameObject.SetActive(false);
        }

        _beards[indxBeards].gameObject.SetActive(true);
    }
    public void MoveToNextHairCut(int direction)
    {
        if (direction > 0)
        {
            if (_tmprHairAccesor.Length - 1 <= indxHairCuts)
            {
                indxHairCuts = 0;
            }
            else
                indxHairCuts++;
        }
        else
        {
            if (indxHairCuts == 0)
            {
                indxHairCuts = _tmprHairAccesor.Length-1;
            }else
                indxHairCuts--;
        }

        if (indxHairCuts > 0 && direction > 0)
        {
            _tmprHairAccesor[indxHairCuts-1].gameObject.SetActive(false);
        }
        else if (direction > 0)
        {
            _tmprHairAccesor[_hairCutsMale.Length-1].gameObject.SetActive(false);
        }
        
        if (direction < 0 && indxHairCuts <_tmprHairAccesor.Length-1)
        {
            _tmprHairAccesor[indxHairCuts+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _tmprHairAccesor[0].gameObject.SetActive(false);
        }

        _tmprHairAccesor[indxHairCuts].gameObject.SetActive(true);
    }

    public void Done()
    {
        if(_playerName.text.Length>0)
            PhotonNetwork.LocalPlayer.NickName = _playerName.text;
        
        if(indxGender==0)
            _settings.Skins.Add(PhotonNetwork.LocalPlayer.NickName,_maleCharacterSkins[indxSkin].name);
        else
            _settings.Skins.Add(PhotonNetwork.LocalPlayer.NickName,_femaleCharacterSkins[indxSkin].name);
        
        _settings.Genders.Add(PhotonNetwork.LocalPlayer.NickName,indxGender);
        _settings.HairCuts.Add(PhotonNetwork.LocalPlayer.NickName,indxHairCuts);
        _settings.Hats.Add(PhotonNetwork.LocalPlayer.NickName,indxHats);
        if(indxGender==1)
            _settings.Beards.Add(PhotonNetwork.LocalPlayer.NickName,0);
        else
            _settings.Beards.Add(PhotonNetwork.LocalPlayer.NickName,indxBeards);

    }

}