using AutoMapper;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.DAL.Persistence;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.DAL.Repositories
{
    public class UserBotRepository : IUserBotRepository
    {
        private readonly IUserBotDao _userDao;
        private readonly IMapper _mapper;

        public UserBotRepository(IUserBotDao userDao, IMapper mapper)
        {
            _userDao = userDao;
            _mapper = mapper;
        }

        public IEnumerable<UserBot> GetAllUsers()
        {
            var usersData = _userDao.GetAllUsers();
            var users = _mapper.Map<List<UserBot>>(usersData);
            return users;
        }

        public void UpsertUser(UserBot userBot)
        {
            var userData = _mapper.Map<UserBotData>(userBot);
            _userDao.UpsertUser(userData);
        }
    }
}
