using UnityEngine;

[CreateAssetMenu(fileName = "New WaterData", menuName = "Water Data")]
public class WaterData : ScriptableObject {
  [Header("Water Properties")]
  public int resolution = 256;
  public int width  = 100;
  public int length = 100;
}
