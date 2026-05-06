using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Infrastructure.Data;

namespace PousadaApi.Application.Services;

public class ReservaService : IReservaService
{
    private readonly PousadaDbContext _dbContext;

    public ReservaService(PousadaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Reserva>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Reservas
            .Include(r => r.Quarto)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .AsQueryable();

        if (pousadaId.HasValue)
        {
            query = query.Where(r => r.Quarto != null && r.Quarto.PousadaId == pousadaId.Value);
        }

        return await query.OrderByDescending(r => r.DataEntrada).ToListAsync(cancellationToken);
    }

    public async Task<Reserva?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservas
            .Include(r => r.Quarto)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Reserva> CriarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        // Verificar se o quarto existe antes de criar a reserva
        var quarto = await _dbContext.Quartos.FirstOrDefaultAsync(q => q.Id == reserva.QuartoId, cancellationToken);
        if (quarto == null) throw new InvalidOperationException("Quarto não encontrado.");

        // Verificar se o quarto está disponível no período solicitado
        var disponivel = await QuartoDisponivelAsync(reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, null, cancellationToken);
        if (!disponivel) throw new InvalidOperationException("Quarto indisponível no período informado.");

        // Calcular o valor total da reserva baseado na quantidade de noites e valor diário
        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;
        reserva.Status = reserva.Status ?? "Confirmada";

        _dbContext.Reservas.Add(reserva);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return reserva;
    }

    public async Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        // Verificar se o quarto existe antes de atualizar a reserva
        var quarto = await _dbContext.Quartos.FirstOrDefaultAsync(q => q.Id == reserva.QuartoId, cancellationToken);
        if (quarto == null) throw new InvalidOperationException("Quarto não encontrado.");

        // Verificar disponibilidade excluindo esta reserva da verificação de conflito
        var disponivel = await QuartoDisponivelAsync(reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, reserva.Id, cancellationToken);
        if (!disponivel) throw new InvalidOperationException("Quarto indisponível no período informado.");

        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;

        _dbContext.Reservas.Update(reserva);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        var reserva = await _dbContext.Reservas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (reserva == null) return;

        reserva.Status = "Cancelada";
        _dbContext.Reservas.Update(reserva);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> QuartoDisponivelAsync(int quartoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdIgnorar = null, CancellationToken cancellationToken = default)
    {
        if (dataEntrada >= dataSaida) return false;

        var query = _dbContext.Reservas
            .Where(r => r.QuartoId == quartoId && r.Status != "Cancelada");

        if (reservaIdIgnorar.HasValue)
            query = query.Where(r => r.Id != reservaIdIgnorar.Value);

        // Há sobreposição de datas quando: data de entrada existente < data de saída nova E data de saída existente > data de entrada nova
        var overlap = await query.AnyAsync(r => r.DataEntrada < dataSaida && r.DataSaida > dataEntrada, cancellationToken);
        return !overlap;
    }
}
