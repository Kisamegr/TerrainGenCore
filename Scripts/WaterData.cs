using UnityEngine;

[CreateAssetMenu(fileName = "New WaterData", menuName = "Water Data")]
public class WaterData : ScriptableObject {
  [Header("Water Properties")]
  public int resolution = 256;
}
