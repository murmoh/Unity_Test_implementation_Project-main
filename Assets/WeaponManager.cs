using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("In-Game Sectors")]
    public GameObject[] slots;
    public GameObject[] equipments;
    public GameObject[] perks;

    [Header("Pre Defined Resources")]
    public GameObject[] weaponList;
    public GameObject[] EquipmentList;
    public GameObject[] perkList;
    [Tooltip("Weapon that is in use will be sent here.")]
    public GameObject Holder;

    [Header("Hand Attach Points")]
    public Transform leftHandTransform;  // Assign the left hand of the arm model here
    public Transform rightHandTransform; // Assign the right hand of the arm model here

    public bool m_slot1;
    public bool m_slot2;
    public bool m_slot3;
    public float currentSlot = 0;

    private Transform leftHandTarget, rightHandTarget; // These will hold our grip point transforms

    void Update()
    {
        Scroll();
        Slot();
        AdjustHandsToGrip();
    }

    void Scroll()
    {
        float y = Input.mouseScrollDelta.y;
        
        if (y > 0) // Scroll up
        {
            currentSlot++;
            if (currentSlot > 3)
                currentSlot = 1;
        }
        else if (y < 0) // Scroll down
        {
            currentSlot--;
            if (currentSlot < 1)
                currentSlot = 3;
        }
    }

    void Start()
    {
        for (int i = 0; i < weaponList.Length && i < slots.Length; i++)
        {
            ManageWeapon(i, i);
        }

        m_slot3 = true; // default slot is 3
        UpdateSlotParenting();  // Ensure the default slot is set as a child of the Holder
    }

    void Slot()
    {
        m_slot1 = (currentSlot == 1);
        m_slot2 = (currentSlot == 2);
        m_slot3 = (currentSlot == 3);

        UpdateSlotParenting();
    }

    void UpdateSlotParenting()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            GameObject slot = slots[i];
            if (i + 1 == currentSlot)
            {
                slot.transform.SetParent(Holder.transform, true); // Move selected slot to Holder
                slot.SetActive(true); // Activate the selected slot

                // Get the active weapon (child of the slot)
                GameObject activeWeapon = slot.transform.GetChild(0).gameObject; 

                // Update hand grip points from the active weapon
                leftHandTarget = activeWeapon.transform.Find("LeftHandGrip");
                rightHandTarget = activeWeapon.transform.Find("RightHandGrip");
            }
            else
            {
                slot.transform.SetParent(this.transform, true); // Move other slots back to the main class object
                slot.SetActive(false); // Deactivate other slots
            }
        }
    }


    void ManageWeapon(int slotIndex, int weaponIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        // Destroy existing weapon in the slot
        if (slots[slotIndex].transform.childCount > 0)
        {
            Destroy(slots[slotIndex].transform.GetChild(0).gameObject);
        }

        // Instantiate new weapon under the slot
        GameObject newWeapon = Instantiate(weaponList[weaponIndex], slots[slotIndex].transform.position, slots[slotIndex].transform.rotation);
        newWeapon.transform.localScale = new Vector3(newWeapon.transform.localScale.x / 4f, newWeapon.transform.localScale.y / 4f, newWeapon.transform.localScale.z / 4f);
        newWeapon.transform.SetParent(slots[slotIndex].transform);
    }

    void AdjustHandsToGrip()
    {
        // This is a simple adjust method; for a smoother transition, you might want to use Lerp or IK
        if(leftHandTarget)
            leftHandTransform.position = leftHandTarget.position;

        if(rightHandTarget)
            rightHandTransform.position = rightHandTarget.position;
    }
}
