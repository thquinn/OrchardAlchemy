using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FruitCostRowScript : MonoBehaviour
{
    public TextMeshProUGUI tmpMass, tmpQuantity;

    public void Init(int mass, int quantity) {
        tmpMass.text = mass.ToString();
        tmpQuantity.text = quantity.ToString();
    }
}
