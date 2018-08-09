using UnityEngine;

[CreateAssetMenu(fileName = "New WaterData", menuName = "Water Data")]
public class WaterData : ScriptableObject {
  [Header("Water Properties")]
  public int size  = 100;
  public int resolution = 256;
}
