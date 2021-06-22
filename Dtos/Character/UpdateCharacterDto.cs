using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
  public class UpdateCharacterDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = "Barney";
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public RpgClass Class { get; set; } = RpgClass.Witch;
}
}