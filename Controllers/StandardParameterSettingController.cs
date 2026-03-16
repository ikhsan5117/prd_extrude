using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class StandardParameterSettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StandardParameterSettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StandardParameterSetting
        public async Task<IActionResult> Index()
        {
            var settings = await _context.StandardParameterSettings
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
            return View(settings);
        }

        // GET: StandardParameterSetting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: StandardParameterSetting/Create
        public IActionResult Create()
        {
            var model = new StandardParameterSetting
            {
                EffectiveDate = DateTime.Today,
                DocumentNumber = "VI-SOP-PROD-131"
            };
            return View(model);
        }

        // POST: StandardParameterSetting/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentNumber,RevisionNumber,CustomerName,LayerType,EffectiveDate,HoseType,Diameter,ProductCode,InnerMaterial,OuterMaterial,YarnType,InnerDie,OuterDie,TubeDie,MiddleDie,CoverDie,SpacerDie,ToleranceDie,Tol_TubeDie,Tol_MiddleDie,Tol_OuterDie,Tol_CoverDie,Tol_SpiralPitch,MeshScreen,HeadTemp,Cylinder1Temp,Cylinder2Temp,Cylinder3Temp,ScrewTemp,ScrewSpeed,FeedRollRatio,Pressure,AirPressureA,PresetValve,SpiralSpeed,SpiralPitch,SpiralSpeedDisplay,SpiralPitchDisplay,PresetTemp,ControlValue,HoseSpeed,TakeupConveyorSpeed,CoolConveyorSpeed,ConveyorRatio,UnsmoothSurface,ChillerWaterTemp,CaterpillarGap,MarkingMaterialColor,MarkingMaterialInner,MarkingMaterialOuter,CreatedBy")] StandardParameterSetting setting)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    setting.CreatedDate = DateTime.Now;
                    setting.IsActive = true;
                    _context.Add(setting);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Standard Parameter Setting berhasil dibuat!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errors = string.Join("<br>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["ErrorMessage"] = "Data tidak valid: <br>" + errors;
                }
            }
            catch (Exception ex)
            {
                var fullMessage = ex.Message;
                if (ex.InnerException != null) {
                    fullMessage += " | Detail: " + ex.InnerException.Message;
                }
                TempData["ErrorMessage"] = "Gagal menyimpan data: " + fullMessage;
                ModelState.AddModelError("", fullMessage);
            }
            return View(setting);
        }

        // GET: StandardParameterSetting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.StandardParameterSettings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: StandardParameterSetting/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentNumber,RevisionNumber,CustomerName,LayerType,EffectiveDate,HoseType,Diameter,ProductCode,InnerMaterial,OuterMaterial,YarnType,InnerDie,OuterDie,TubeDie,MiddleDie,CoverDie,SpacerDie,ToleranceDie,Tol_TubeDie,Tol_MiddleDie,Tol_OuterDie,Tol_CoverDie,Tol_SpiralPitch,MeshScreen,HeadTemp,Cylinder1Temp,Cylinder2Temp,Cylinder3Temp,ScrewTemp,ScrewSpeed,FeedRollRatio,Pressure,AirPressureA,PresetValve,SpiralSpeed,SpiralPitch,SpiralSpeedDisplay,SpiralPitchDisplay,PresetTemp,ControlValue,HoseSpeed,TakeupConveyorSpeed,CoolConveyorSpeed,ConveyorRatio,UnsmoothSurface,ChillerWaterTemp,CaterpillarGap,MarkingMaterialColor,MarkingMaterialInner,MarkingMaterialOuter,IsActive")] StandardParameterSetting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            var existingSetting = await _context.StandardParameterSettings.FindAsync(id);
            if (existingSetting == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Map basic fields
                    existingSetting.DocumentNumber = setting.DocumentNumber;
                    existingSetting.RevisionNumber = setting.RevisionNumber;
                    existingSetting.CustomerName = setting.CustomerName;
                    existingSetting.LayerType = setting.LayerType;
                    existingSetting.EffectiveDate = setting.EffectiveDate;
                    existingSetting.HoseType = setting.HoseType;
                    existingSetting.Diameter = setting.Diameter;
                    existingSetting.ProductCode = setting.ProductCode;
                    existingSetting.InnerMaterial = setting.InnerMaterial;
                    existingSetting.OuterMaterial = setting.OuterMaterial;
                    existingSetting.YarnType = setting.YarnType;
                    
                    // Map dimension fields
                    existingSetting.InnerDie = setting.InnerDie;
                    existingSetting.OuterDie = setting.OuterDie;
                    existingSetting.TubeDie = setting.TubeDie;
                    existingSetting.MiddleDie = setting.MiddleDie;
                    existingSetting.CoverDie = setting.CoverDie;
                    existingSetting.SpacerDie = setting.SpacerDie;
                    existingSetting.ToleranceDie = setting.ToleranceDie;
                    existingSetting.Tol_TubeDie = setting.Tol_TubeDie;
                    existingSetting.Tol_MiddleDie = setting.Tol_MiddleDie;
                    existingSetting.Tol_OuterDie = setting.Tol_OuterDie;
                    existingSetting.Tol_CoverDie = setting.Tol_CoverDie;
                    existingSetting.Tol_SpiralPitch = setting.Tol_SpiralPitch;
                    existingSetting.SpiralPitch = setting.SpiralPitch;



                    // Map process fields
                    existingSetting.HeadTemp = setting.HeadTemp;
                    existingSetting.Cylinder1Temp = setting.Cylinder1Temp;
                    existingSetting.Cylinder2Temp = setting.Cylinder2Temp;
                    existingSetting.Cylinder3Temp = setting.Cylinder3Temp;
                    existingSetting.ScrewTemp = setting.ScrewTemp;
                    existingSetting.ScrewSpeed = setting.ScrewSpeed;
                    existingSetting.FeedRollRatio = setting.FeedRollRatio;
                    existingSetting.Pressure = setting.Pressure;
                    existingSetting.PresetTemp = setting.PresetTemp;
                    existingSetting.HoseSpeed = setting.HoseSpeed;
                    existingSetting.IsActive = setting.IsActive;

                    _context.Update(existingSetting);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Standard Parameter Setting berhasil diupdate!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StandardParameterSettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Gagal mengupdate data: " + ex.Message);
                }
            }
            return View(setting);
        }

        // GET: StandardParameterSetting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: StandardParameterSetting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setting = await _context.StandardParameterSettings.FindAsync(id);
            if (setting != null)
            {
                // Soft delete
                setting.IsActive = false;
                _context.Update(setting);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Standard Parameter Setting berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StandardParameterSettingExists(int id)
        {
            return _context.StandardParameterSettings.Any(e => e.Id == id);
        }

        // GET: Cetak form parameter setting
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }
        [HttpGet]
        public async Task<IActionResult> GetMasterDataByItem(string itemCode)
        {
            if (string.IsNullOrEmpty(itemCode)) return Json(null);

            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList == itemCode);

            if (master == null) return Json(null);

            return Json(master);
        }
    }
}
