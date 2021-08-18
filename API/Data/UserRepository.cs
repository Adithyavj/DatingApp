using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                    .Where(u => u.UserName == username)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
        }

        // only return required no. of results as reponse to show them as pages
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking(); // turn off tracking.

            // passes the data to pagedList class method execute the query to get data from db (total record count and the results how much to get
            // based on the pagenumber and pagesize ).
            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                        .Include(p => p.Photos)
                        .SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                        .Include(p => p.Photos)
                        .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // if > 1 changes have been made
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // this lets entity framework track this
        }
    }
}