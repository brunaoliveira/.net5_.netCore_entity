using System.Threading.Tasks;
using dotnet_rpg.Models;

namespace dotnet_rpg.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;

    public AuthRepository(DataContext context)
    {
      _context = context;
    }

    public Task<ServiceResponse<string>> Login(string username, string password)
    {
      throw new System.NotImplementedException();
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        ServiceResponse<int> response = new ServiceResponse<int>();
        response.Data = user.Id;
        
        return response;
    }

    public Task<bool> UserExists(string username)
    {
      throw new System.NotImplementedException();
    }
  }
}