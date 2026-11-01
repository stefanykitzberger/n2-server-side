using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IMedicoService
{
    Task<List<MedicoReadDTO>> GetAllAsync();

    Task<MedicoReadDTO?> GetByIdAsync(int id);

    Task<MedicoReadDTO> CreateAsync(MedicoCreateDTO dto);

    Task<bool> UpdateAsync(int id, MedicoUpdateDTO dto);

    Task<bool> DeleteAsync(int id);
}
