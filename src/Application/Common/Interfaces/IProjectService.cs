namespace PPyrekBackend15043.Application.Common.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateAsync(ProjectDto dto);
        Task<ProjectDto> UpdateAsync(ProjectDto dto);
        Task DeleteAsync(Guid id);
        Task<ProjectDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ProjectDto>> GetAllAsync();
    }

    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
