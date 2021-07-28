using System;
using System.Threading.Tasks;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.Fight
{
  public class FightService : IFightService
  {
    private readonly DataContext _context;

    public FightService(DataContext context)
    {
      _context = context;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
      var response = new ServiceResponse<AttackResultDto>();

      try
      {
        var atacker = await _context.Characters
          .Include(c => c.Weapon)
          .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

        var opponent = await _context.Characters
          .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

        int damage = atacker.Weapon.Damage + (new Random().Next(atacker.Strength)); // use the attacker strenght
        damage -= new Random().Next(opponent.Defense); // use the "atackee" defense

        if (damage > 0)
          opponent.HitPoints -= damage;

        if (opponent.HitPoints <= 0)
          response.Message = $"{opponent.Name} has been defeated!";

        await _context.SaveChangesAsync();

        response.Data = new AttackResultDto
        {
          AttackerName = atacker.Name,
          AttackerHP = atacker.HitPoints,
          OpponentName = opponent.Name,
          OpponentHP = opponent.HitPoints,
          Damage = damage
        };
      }
      catch (Exception ex)
      {
        response.Success = false;
        response.Message = ex.Message;
      }

      return response;
    }
  }
}