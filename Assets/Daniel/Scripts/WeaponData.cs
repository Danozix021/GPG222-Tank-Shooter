using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;

    [Header("Firing")]
    public bool fullAuto = false;
    public float fireRate = 4f;          
    public int bulletsPerShot = 1;       
    public float spreadAngle = 0f;       

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;
    public int damage = 20;
    public float bulletLifeTime = 2f;

}