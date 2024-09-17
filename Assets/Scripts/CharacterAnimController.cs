using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using System.Collections;

public class CharacterAnimController : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] Animator _tppModelAnimator;
    [SerializeField] Animator _fppModelAnimator;
    [SerializeField] RuntimeAnimatorController _baseAnimatorController;
    [SerializeField] RuntimeAnimatorController _baseAnimatorControllerFPP;

    [Header("ListResources")]
    [SerializeField] List<RuntimeAnimatorController> runtimeAnimatorControllers;
    [SerializeField] List<RuntimeAnimatorController> runtimeFppAnimatorControllers;
    [SerializeField] List<string> controllerNames;
    [SerializeField] List<GameObject> equipments;
    [SerializeField] List<GameObject> heroes;

    [Header("UIs")]
    [SerializeField] Text ControllerName;

    [Header("Btns")]
    [SerializeField] Button nextBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button tppBtn;
    [SerializeField] Button fppBtn;
    [SerializeField] Button heroChangeBtn;

    [Header("GObjs")]
    [SerializeField] GameObject tppObj;
    [SerializeField] GameObject fppObj;

    [Header("Camera")]
    [SerializeField] Transform camtrans;
    [SerializeField] Button camLeftBtn;
    [SerializeField] Button camRightBtn;
    [SerializeField] Button camBotBtn;
    [SerializeField] Button camTopBtn;
    [SerializeField] Button upCamBtn;
    [SerializeField] Button downCamBtn;
    [SerializeField] float heighStepCam = 1;

    [Header("ItemPos")]
    [SerializeField] Transform itemTargetPos;
    [SerializeField] Transform itemParent;
    [SerializeField] Transform itemFppTargetPos;
    [SerializeField] Vector3 itemRotatonCorrecter = new Vector3(-90, -90, -90);

    [Header("Character")]
    [SerializeField] CharacterController characterController;
    [SerializeField] CameraAnimController cameraAnimController;

    private int cIndexController = 0;
    private int maxIndexController = 0;
    private bool isMovingCam = false;
    private Vector3 leftCamPos = new Vector3(5, 4, 0);
    private Vector3 rightCamPos = new Vector3(-5, 4, 0);
    private Vector3 botCamPos = new Vector3(0, 4, -5);
    private Vector3 topCamPos = new Vector3(0, 4, 5);
    private GameObject cItem;
    private Vector3 fppItemRotatonCorrecter = new Vector3(-90, -90, 0);
    private bool isTpp = true;
    private bool isSkilling = false;
    private int cHeroIdx = 0;
    private int heroCount = 0;

    private void Awake()
    {
        heroCount = heroes.Count;
        maxIndexController = runtimeAnimatorControllers.Count-1;

        nextBtn?.onClick.AddListener(() => 
        {
            cIndexController++;
            cIndexController = cIndexController > maxIndexController ? 0 : cIndexController;
            if(isTpp)
                SetController();
            else
                SetFppController();
        });

        backBtn?.onClick.AddListener(() =>
        {
            cIndexController--;
            cIndexController = cIndexController < 0 ? maxIndexController : cIndexController;
            if(isTpp)
                SetController();
            else
                SetFppController();
        });

        tppBtn?.onClick.AddListener(() =>
        {
            isTpp = true;
            OnchangeModel(isTpp);
        });

        fppBtn?.onClick.AddListener(() =>
        {
            isTpp = false;
            OnchangeModel(isTpp);
        });

        camLeftBtn?.onClick.AddListener(() =>
        {
            OnMoveCam(leftCamPos);
        });

        camRightBtn?.onClick.AddListener(() =>
        {
            OnMoveCam(rightCamPos);
        });

        camBotBtn?.onClick.AddListener(() =>
        {
            OnMoveCam(botCamPos);
        });

        camTopBtn?.onClick.AddListener(() =>
        {
            OnMoveCam(topCamPos);
        });

        upCamBtn?.onClick.AddListener(() =>
        {
            OnUpDownCam(heighStepCam);
        });

        downCamBtn?.onClick.AddListener(() =>
        {
            OnUpDownCam(-heighStepCam);
        });

        heroChangeBtn?.onClick.AddListener(() =>
        {
            heroes[cHeroIdx].SetActive(false);
            cHeroIdx = cHeroIdx + 1 >= heroCount ? 0 : cHeroIdx + 1;
            heroes[cHeroIdx].SetActive(true);
        });

        InitEquipsList();
    }

    void Update()
    {
        camtrans.LookAt(transform.position);
        InputController();

        if(_movementInput != Vector2.zero)
        {
            cameraAnimController.CCamState = CamState.CamMoving;
            var moment = Vector3.zero;
            var mar = Mathf.Sqrt(_movementInput.x * _movementInput.x + _movementInput.y * _movementInput.y);
            moment.z = _movementInput.y/mar;
            moment.x = _movementInput.x / mar;

            var angle = Quaternion.LookRotation(moment, Vector3.up);
            transform.localRotation = Quaternion.Lerp(transform.rotation, angle, Time.deltaTime * 10);
            characterController.Move(moment*3*Time.deltaTime);
        }
        else
        {
            _tppModelAnimator.SetFloat("speed", 0);
            cameraAnimController.CCamState = CamState.CamDefault;
        }
        _movementInput = Vector2.zero;
    }

    private void OnEnable()
    {
        OnchangeModel(isTpp);
    }

    private void OnUpDownCam(float value)
    {
        if(camtrans != null)
        {
            var newPos = camtrans.localPosition + Vector3.up * value;
            newPos.y = Mathf.Clamp(newPos.y, 2f, 6.5f);
            camtrans.localPosition = newPos;
        }
    }

    private void InitEquipsList()
    {
        foreach (var eq in equipments) 
        {
            eq.SetActive(false);
        }
    }

    private void OnMoveCam(Vector3 target)
    {
        if(isMovingCam) return; 
        
        isMovingCam = true;
        camtrans.DOLocalMove(target, 1f).SetEase(Ease.OutBack).OnComplete(() => isMovingCam = false);
    }

    private void OnchangeModel(bool isTpp)
    {
        tppObj?.SetActive(isTpp);
        fppObj?.SetActive(!isTpp);

        if(isTpp)
            SetController();
        else
            SetFppController();
    }

    private void SetController()
    {
        _baseAnimatorController = runtimeAnimatorControllers[cIndexController];
        _tppModelAnimator.runtimeAnimatorController = _baseAnimatorController;
        AssignItem();
        SetControllerName();
    }

    private void SetFppController()
    {
        //_baseAnimatorControllerFPP = runtimeFppAnimatorControllers[cIndexController];
        _fppModelAnimator.runtimeAnimatorController = _baseAnimatorControllerFPP;
        AssignFppItem();
        SetControllerName();
    }

    private void AssignItem()
    {
        ResolveOldItem();
        ResolveNewItem();
    }

    private void AssignFppItem()
    {
        ResolveOldFppItem();
        ResolveNewFppItem();
    }

    private void ResolveNewItem()
    {
        cItem = equipments[cIndexController];
        cItem.transform.SetParent(itemTargetPos);
        cItem.transform.position = itemTargetPos.position;
        cItem.transform.rotation = itemTargetPos.rotation;
        cItem.transform.Rotate(itemRotatonCorrecter);
        cItem.SetActive(true);
    }

    private void ResolveNewFppItem()
    {
        cItem = equipments[cIndexController];
        cItem.transform.SetParent(itemFppTargetPos);
        cItem.transform.position = itemFppTargetPos.position;
        cItem.transform.rotation = itemFppTargetPos.rotation;
        cItem.transform.Rotate(fppItemRotatonCorrecter);
        cItem.SetActive(true);
    }

    private void ResolveOldItem()
    {
        if(cItem == null) return;

        var oldItem = cItem;
        oldItem.transform.SetParent(itemParent);
        oldItem.transform.position = Vector3.zero;
        oldItem.transform.rotation = quaternion.identity;
        oldItem.SetActive(false);
    }

    private void ResolveOldFppItem()
    {
        if (cItem == null) return;

        var oldItem = cItem;
        oldItem.transform.SetParent(itemParent);
        oldItem.transform.position = Vector3.zero;
        oldItem.transform.rotation = quaternion.identity;
        oldItem.SetActive(false);
    }

    private void SetControllerName()
    {
        ControllerName.text = controllerNames[cIndexController];
    }

    private Vector2 _movementInput;
    private void InputController()
    {
        if (isTpp)
        {
            if (Input.GetKey(KeyCode.A))
            {
                _movementInput.x = -1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _movementInput.x = 1;
            }

            if (Input.GetKey(KeyCode.W))
            {
                _movementInput.y = 1;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                _movementInput.y = -1;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl)) _tppModelAnimator.SetBool("isCrouching", true);
            if (Input.GetKeyUp(KeyCode.LeftControl)) _tppModelAnimator.SetBool("isCrouching", false);

            if (_movementInput != Vector2.zero)
            {
                _tppModelAnimator.SetFloat("X", 0);
                _tppModelAnimator.SetFloat("Y", 1);
                _tppModelAnimator.SetFloat("speed", 1);
            }

            if (Input.GetKeyDown(KeyCode.Space)) _tppModelAnimator.SetBool("isGrounded", false);
            if (Input.GetKeyUp(KeyCode.Space)) _tppModelAnimator.SetBool("isGrounded", true);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))  _fppModelAnimator.SetFloat("speed", 2);
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) _fppModelAnimator.SetFloat("speed", 0);

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (isSkilling) return;

                isSkilling = true;
                _fppModelAnimator.SetTrigger("isActionSkill");
                StartCoroutine(DoActionSkill(0));
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (isSkilling) return;

                isSkilling = true;
                _fppModelAnimator.SetBool("isActionSkill", isSkilling);
                StartCoroutine(DoActionSkill(1));
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (isSkilling) return;

                isSkilling = true;
                _fppModelAnimator.SetBool("isActionSkill", isSkilling);
                StartCoroutine(DoActionSkill(2));
            }
        }
    }

    private IEnumerator DoActionSkill(float skillType)
    {
        cItem.SetActive(false);
        Vector3 skillState = skillType == 0 ? new Vector3(1.1f, 2.1f * 3f, 0.8f) : (skillType == 1 ? new Vector3(1.6f, 6f, 0.733f) : new Vector3(1f, 6f, 1.267f));
        _fppModelAnimator.SetFloat("skill_type", skillType);
        _fppModelAnimator.SetFloat("skill_state", 0);
        yield return new WaitForSeconds(skillState.x);
        _fppModelAnimator.SetFloat("skill_state", 1);
        yield return new WaitForSeconds(skillState.y);
        _fppModelAnimator.SetFloat("skill_state", 2);
        yield return new WaitForSeconds(skillState.z);
        isSkilling = false;
        _fppModelAnimator.SetTrigger("IsEndSkill");
        cItem.SetActive(true);
    }
}
