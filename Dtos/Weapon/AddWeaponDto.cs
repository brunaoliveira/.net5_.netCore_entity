namespace dotnet_rpg.Services.WeaponService
{
  public class AddWeaponDto
  {
    public string Name { get; set; }
    public int Damage { get; set; }
    public int CharacterId { get; set; }
  }
}