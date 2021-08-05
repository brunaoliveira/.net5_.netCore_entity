using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
    {
      var response = new ServiceResponse<FightResultDto>
      {
        Data = new FightResultDto()
      };

      try
      {
        var characters = await _context.Characters
          .Include(c => c.Weapon)
          .Include(c => c.Skills)
          .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

        bool defeated = false;

        while (!defeated)
        {
          foreach (var attacker in characters)
          {
            var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
            var opponent = opponents[new Random().Next(opponents.Count)];
            var damage = 0;
            string attackUsed = string.Empty;
            bool useWeapon = new Random().Next(2) == 0;

            if (useWeapon)
            {
              attackUsed = attacker.Weapon.Name;
              damage = DoWeaponAttack(attacker, opponent);
            }
            else
            {
              var skill = opponent.Skills[new Random().Next(opponent.Skills.Count)];
              attackUsed = skill.Name;
              damage = DoSkillAttack(attacker, opponent, skill);
            }

            response.Data.Log
              .Add($"{attacker.Name} atacks {opponent.Name} using {attackUsed}, with {(damage > 0 ? damage : 0)} damage");

            if (opponent.HitPoints <= 0)
            {
              defeated = true;
              attacker.Victories++;
              opponent.Defeats++;

              response.Data.Log.Add($"{opponent.Name} has been defeated.");
              response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");

              break;
            }
          }
        }

        characters.ForEach(c => {
          c.Fights++;
          c.HitPoints = 100;
        });

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        response.Success = false;
        response.Message = ex.Message;
      }

      return response;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
      var response = new ServiceResponse<AttackResultDto>();

      try
      {
        var attacker = await _context.Characters
          .Include(c => c.Skills)
          .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

        var opponent = await _context.Characters
          .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

        var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

        if (skill == null)
        {
          response.Success = false;
          response.Message = $"{attacker.Name} doesn't know this skill.";

          return response;
        }

        int damage = DoSkillAttack(attacker, opponent, skill);

        if (opponent.HitPoints <= 0)
          response.Message = $"{opponent.Name} has been defeated!";

        response.Data = new AttackResultDto
        {
          AttackerName = attacker.Name,
          AttackerHP = attacker.HitPoints,
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

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
      var response = new ServiceResponse<AttackResultDto>();

      try
      {
        var attacker = await _context.Characters
          .Include(c => c.Weapon)
          .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

        var opponent = await _context.Characters
          .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

        int damage = DoWeaponAttack(attacker, opponent);

        if (opponent.HitPoints <= 0)
          response.Message = $"{opponent.Name} has been defeated!";

        await _context.SaveChangesAsync();

        response.Data = new AttackResultDto
        {
          AttackerName = attacker.Name,
          AttackerHP = attacker.HitPoints,
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

    private static int DoWeaponAttack(Character atacker, Character opponent)
    {
      int damage = atacker.Weapon.Damage + (new Random().Next(atacker.Strength)); // use the attacker strenght
      damage -= new Random().Next(opponent.Defense); // use the "atackee" defense

      if (damage > 0)
        opponent.HitPoints -= damage;

      return damage;
    }

    private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
    {
      var damage = skill.Damage + (new Random().Next(attacker.Intelligence));
      damage -= new Random().Next(opponent.Defense);

      if (damage > 0)
        opponent.HitPoints -= damage;

      return damage;
    }
  }
}