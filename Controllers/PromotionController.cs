using Microsoft.AspNetCore.Mvc;
using PromoPrizeApi.EditModels;
using PromoPrizeApi.Entities;

namespace PromoPrizeApi.Controllers;

[ApiController]
[Route("promo")]
public class WeatherForecastController : ControllerBase
{
    private readonly PromotionService promotionService;
    private readonly Random random = new();

    public WeatherForecastController(PromotionService promotionService)
    {
        this.promotionService = promotionService;
    }

    /// <summary>
    /// Создать промоакцию.
    /// </summary>
    /// <param name="promotion">Промоакция</param>     
    [HttpPost]
    public IActionResult Post([FromBody] EditPromotion promotion)
    {
        if (string.IsNullOrWhiteSpace(promotion.Name))
            return BadRequest("Название должно быть указано!");
        var id = promotionService.promotions.Count;
        promotionService.promotions.Add(new Promotion
        {
            Name = promotion.Name,
            Description = promotion.Description,
            Id = id
        });
        return Ok(id);
    }

    /// <summary>
    /// Получить базовую информацию обо всех промоакциях.
    /// </summary>
    [HttpGet]
    public IActionResult GetBasePromos()
    {
        return Ok(promotionService.promotions.Select(x => new {id = x.Id, name = x.Name, description = x.Description}));
    }

    /// <summary>
    /// Получить полную информацию о промоакции.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>     
    [HttpGet("{id:int}")]
    public IActionResult GetPromo(int id)
    {
        var promotion = promotionService.promotions.FirstOrDefault(x => x.Id == id);
        if (promotion is null)
            return NotFound();

        return Ok(promotion);
    }

    /// <summary>
    /// Обновить промоакцию.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>
    /// <param name="editPromotion">Новые данные</param>
    [HttpPut("{id:int}")]
    public IActionResult UpdatePromo(int id, [FromBody] EditPromotion editPromotion)
    {
        if (string.IsNullOrWhiteSpace(editPromotion.Name))
            return BadRequest("Название должно быть указано!");

        var promotionIdx = promotionService.promotions.FindIndex(x => x.Id == id);
        if (promotionIdx < 0)
            return NotFound();

        promotionService.promotions[promotionIdx] = new Promotion()
        {
            Name = editPromotion.Name,
            Description = editPromotion.Description,
            Id = promotionIdx
        };

        return Ok();
    }

    /// <summary>
    /// Удалить промоакцию.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>
    [HttpDelete("{id:int}")]
    public IActionResult DeletePromo(int id)
    {
        var removed = promotionService.promotions.RemoveAll(x => x.Id == id);
        return removed > 0 ? Ok() : NotFound();
    }

    /// <summary>
    /// Добавить участника в промоакцию.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>
    /// <param name="participant">Участник</param>
    [HttpPost("{id:int}/participant")]
    public IActionResult AddParticipant(int id, [FromBody] Participant participant)
    {
        var promotionIdx = promotionService.promotions.FindIndex(x => x.Id == id);
        if (promotionIdx < 0)
            return NotFound();

        var promotion = promotionService.promotions[promotionIdx];
        participant.Id = promotion.Participants.Count;
        promotion.Participants.Add(participant);
        return Ok(participant.Id);
    }

    /// <summary>
    /// Удалить участника из промоакции.
    /// </summary>
    /// <param name="promoId">Идентификатор промоакции</param>
    /// <param name="participantId">Идентификатор участника</param>
    [HttpDelete("{promoId:int}/participant/{participantId:int}")]
    public IActionResult DeletePromoParticipant(int promoId, int participantId)
    {
        var promotionIdx = promotionService.promotions.FindIndex(x => x.Id == promoId);
        if (promotionIdx < 0)
            return NotFound("Промоакция не найдена");

        var promotion = promotionService.promotions[promotionIdx];
        var removed = promotion.Participants.RemoveAll(x => x.Id == participantId);
        return removed > 0 ? Ok() : NotFound();
    }

    /// <summary>
    /// Добавить приз в промоакцию.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>
    /// <param name="prize">Приз</param>
    [HttpPost("{id:int}/prize")]
    public IActionResult DeletePromo(int id, [FromBody] Prize prize)
    {
        var promotionIdx = promotionService.promotions.FindIndex(x => x.Id == id);
        if (promotionIdx < 0)
            return NotFound();

        var promotion = promotionService.promotions[promotionIdx];
        prize.Id = promotion.Prizes.Count;
        promotion.Prizes.Add(prize);
        return Ok(prize.Id);
    }

    /// <summary>
    /// Удалить приз из промоакции.
    /// </summary>
    /// <param name="promoId">Идентификатор промоакции</param>
    /// <param name="prizeId">Идентификатор приза</param>
    [HttpDelete("{promoId:int}/prize/{prizeId:int}")]
    public IActionResult DeletePromoPrize(int promoId, int prizeId)
    {
        var promotionIdx = promotionService.promotions.FindIndex(x => x.Id == promoId);
        if (promotionIdx < 0)
            return NotFound("Промоакция не найдена");

        var promotion = promotionService.promotions[promotionIdx];
        var removed = promotion.Prizes.RemoveAll(x => x.Id == prizeId);
        return removed > 0 ? Ok() : NotFound();
    }

    /// <summary>
    /// Провести розыгрыш.
    /// </summary>
    /// <param name="id">Идентификатор промоакции</param>
    [HttpPost("{id:int}/raffle")]
    public IActionResult RafflePromo(int id)
    {
        var promotion = promotionService.promotions.FirstOrDefault(x => x.Id == id);
        if (promotion is null)
            return NotFound();

        if (promotion.Prizes.Count != promotion.Participants.Count)
            return Conflict();

        var stack = new Stack<Prize>(promotion.Prizes.OrderBy(_ => random.Next()));
        var results = promotion.Participants.Select(x =>
            new {winner = x.Name, prize = stack.Pop()}).ToList();
        return Ok(results);
    }
}