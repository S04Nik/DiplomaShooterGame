using System.Collections;
using Com.Tereshchuk.Shooter;
using Photon.Pun;
using UnityEngine;

public class ActiveWeapon : MonoBehaviourPunCallbacks,IPunObservable
{
    public Transform[] weaponSlots;
    public Animator rigController;
    public CharacterAiming characterAiming;
    public Gun[] loadOut;

    private bool _isHolstered;
    private bool _isChangingWeapon;
    private RaycastWeapon[] _equipedWeapons = new RaycastWeapon[2];
    private int _activeWeaponIndex;
    private AmmoWidget _ammoWidget;
    private Transform _canvasHitSuccess;
    [SerializeField] private WeaponSoundController _soundController;
    [SerializeField] private ReloadWeapon _reloadWeapon;
    
    private readonly int _weaponIndxParam = Animator.StringToHash("WeaponIndex");
    private readonly int _holsterWeaponParam = Animator.StringToHash("Holster_Weapon");
    private readonly int _notSprintingParam = Animator.StringToHash("notSprinting");


    public bool IsFiring()
    {
        RaycastWeapon activeWeapon = GetActiveWeapon();
        if (activeWeapon)
        {
            return activeWeapon.isFiring;
        }
        else
            return false;
    }
    public bool IsChangingWeapon()
    {
        return _isChangingWeapon;
    }

    public bool GetHolsteredState()
    {
        return _isHolstered;
    }
    RaycastWeapon GetWeapon(int indx)
    {
        if (indx < 0 || indx > _equipedWeapons.Length)
        {
            return null;
        }
        return _equipedWeapons[indx];
    }
    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(_activeWeaponIndex);
    }

    public void SetCanvasSuccesHitImg(Transform succesHitCanvasImg)
    {
        _canvasHitSuccess = succesHitCanvasImg;
    }
    void Start()
    {
        // RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        _ammoWidget = GameObject.Find("HUD/Ammo").GetComponent<AmmoWidget>();
        // if (existingWeapon)
        // {
        //     Equip(existingWeapon);
        // }

    }   

    public IEnumerator ChangeCanvasAimingDot()
    {
        _canvasHitSuccess.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _canvasHitSuccess.gameObject.SetActive(false);
        StopCoroutine(nameof(ChangeCanvasAimingDot));
    }

    private void StartChangingCanvasAimindDot()
    {
        StartCoroutine(nameof(ChangeCanvasAimingDot));
    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            var weapon = GetWeapon(_activeWeaponIndex);
            bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash == _notSprintingParam;
        
            if (weapon && !_isHolstered && notSprinting && !_reloadWeapon.GetReloadingState())
            {
                weapon.UpdateWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                photonView.RPC("SetActiveWeapon",RpcTarget.All,0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                photonView.RPC("SetActiveWeapon",RpcTarget.All,1);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                photonView.RPC("ToggleActiveWeapon",RpcTarget.All);
            }
        }
       
    }
    //ViewId (null/0) so the networked object is not created by an actor and is instead associated with the room.
    //photonView.IsMine indicates if the PhotonNetwork.LocalPlayer is the current Controller for this photonView.
    [PunRPC]
    public void ShowOthersMyWeapon(int weaponId,int ownerId)
    {

        //RaycastWeapon newWeapon = Instantiate(loadOut[loadOutIndex].prefab).GetComponent<RaycastWeapon>();
        RaycastWeapon newWeapon = PhotonView.Find(weaponId).GetComponent<RaycastWeapon>();

        //newWeapon.SetCamera(characterAiming._cameraMain);   
        int weaponSlotIndex = newWeapon.loadOut.slot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
        {
            Destroy(weapon.gameObject);//raycastweapon not gameobject
        }
        weapon = newWeapon;
        weapon.recoil.SetRecoil(characterAiming,rigController);

        weapon.transform.SetParent(weaponSlots[weaponSlotIndex],false);
        _equipedWeapons[weaponSlotIndex] = weapon;

        //photonView.RPC("SetActiveWeapon",RpcTarget.All,newWeapon.loadOut.slot);
        SetActiveWeapon(newWeapon.loadOut.slot);
    }
    public void Equip(int loadOutIndex,PhotonView view)
    {
       if (photonView.IsMine)
       {

           RaycastWeapon newWeapon = PhotonNetwork.Instantiate(loadOut[loadOutIndex].prefab.name,Vector3.zero,Quaternion.identity).GetComponent<RaycastWeapon>();
           
           newWeapon.ChangeCanvasAimingDot += StartChangingCanvasAimindDot;
           
           newWeapon.SetCamera(characterAiming.GetCameraMain());
           newWeapon.SetAudioController(_soundController);

           if (photonView.ViewID == view.ViewID)
           {

               newWeapon.photonView.RequestOwnership();
           }
           photonView.RPC(nameof(ShowOthersMyWeapon),RpcTarget.AllBuffered,newWeapon.photonView.ViewID,view.ViewID);
       }

    }
    
    [PunRPC]
    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool(_holsterWeaponParam);
        if (isHolstered)
        {
             StartCoroutine(ActivateWeapon(_activeWeaponIndex));
        }
        else
        {
             StartCoroutine(HolsterWeapon(_activeWeaponIndex));
        }
    }

    [PunRPC]
    void SetActiveWeapon(int weaponSlotIndex)
    {
        int holsterIndex = _activeWeaponIndex;
        int activateIndex = weaponSlotIndex;

        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1; // HolsterWeapon = null
        }
        
        StartCoroutine(SwitchWeapon(holsterIndex,activateIndex));
    }
    
    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        rigController.SetInteger(_weaponIndxParam,activateIndex);
        
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        //yield return new WaitForSeconds(1f); 
        // не дожидает окончание holster = false
        yield return StartCoroutine(ActivateWeapon(activateIndex));

        _activeWeaponIndex = activateIndex;
        
        _ammoWidget.RefreshAmmo(_equipedWeapons[_activeWeaponIndex].loadOut.GetClip(),_equipedWeapons[_activeWeaponIndex].loadOut.GetStash());
        StopCoroutine("SwitchWeapon");
    }

    IEnumerator HolsterWeapon(int indx)
    {
        _isChangingWeapon = true;
        _isHolstered = true;
        var weapon = GetWeapon(indx);
        if (weapon)
        {
            rigController.SetBool(_holsterWeaponParam,true);
            // do
            // {
                // не успевает закончится . срабатывает 1 раз
                yield return new WaitForSeconds(1);
            // } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            
            _isChangingWeapon = false;
            
            StopCoroutine("HolsterWeapon");
        }
    }

    IEnumerator ActivateWeapon(int indx)
    {
        _isChangingWeapon = true;

        var weapon = GetWeapon(indx);
        if (weapon)
        {
            rigController.SetBool(_holsterWeaponParam,false);
            rigController.Play("equip_"+weapon.loadOut.name);
            // do
            // {
            //     yield return new WaitForEndOfFrame();
            // } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            yield return new WaitForSeconds(1);

            _isHolstered = false;
            _isChangingWeapon = false;
            
            StopCoroutine("ActivateWeapon");
        }
        
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // stream.SendNext(_equipedWeapons[_activeWeaponIndex]);
        }
        else
        {
            // _equipedWeapons[_activeWeaponIndex]=(RaycastWeapon)stream.ReceiveNext();
        }
    }
}

