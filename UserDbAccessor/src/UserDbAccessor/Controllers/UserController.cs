using Dapr.Client;





namespace UserDbAccessor.Controllers
{
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly DaprClient _daprClient;

		public UserController(IUserRepository userRepository, DaprClient daprClient)
		{
			_userRepository = userRepository;
			_daprClient = daprClient;

		}

		[HttpGet("api/user")]
		public async Task<IEnumerable<User>> GetAllUsers()
		{
			return await _userRepository.GetAllUsers();
		}

		[HttpGet("api/user/{id}")]
		public async Task<ActionResult<User>> GetUserById(int id)
		{
			var user = await _userRepository.GetUserById(id);
			if (user == null)
			{
				return NotFound();
			}
			return user;
		}



		[HttpPost("api/user")]
		public async Task<ActionResult<User>> AddUser([FromBody] User user)
		{
			var createdUser = await _userRepository.AddUser(user);

			CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken cancellationToken = source.Token;
			using var client = new DaprClientBuilder().Build();
			Console.WriteLine(createdUser.ToString());
			
			await _daprClient.PublishEventAsync("pubsub", "userRegisterDetails", createdUser);

            Console.WriteLine("User Send");

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
			
		}

		[HttpPut("api/user/{id}/preferences")]
		public async Task<ActionResult<User>> UpdateUserPreferences(int id, string preferences)
		{
			var updatedUser = await _userRepository.UpdateUserPreferences(id, preferences);
			if (updatedUser == null)
			{
				return NotFound();
			}
			return updatedUser;
		}

		[HttpDelete("api/user/{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var result = await _userRepository.DeleteUser(id);
			if (!result)
			{
				return NotFound();
			}
			return NoContent();
		}
	}
}