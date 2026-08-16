using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TrainingCenterFinalProject.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PetCareDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<PetCareDbContext>>()))
            {
                var now = DateTime.Now;

                // ==========================================
                // ★ 獨立區塊：檢查並建立 職位 (Occupations)
                // ==========================================
                if (!context.Occupations.Any())
                {
                    var occAdmin = new Occupations { OccupationID = "O00", OccupationName = "獸醫管理" };
                    var occVet = new Occupations { OccupationID = "O01", OccupationName = "獸醫師" };
                    var occVetDefault = new Occupations { OccupationID = "O09", OccupationName = "預設獸醫師" };
                    var occGroomer = new Occupations { OccupationID = "O11", OccupationName = "寵物美容師" };
                    
                    context.Occupations.AddRange(occVet, occGroomer, occAdmin,occVetDefault);
                    context.SaveChanges();
                }

                // ==========================================
                // ★ 獨立區塊：檢查並建立 就業紀錄 (Employment)
                // ==========================================
                // 若剛剛在資料庫手動清空了 Employment，這裡就會偵測到沒有資料並重新寫入
                if (!context.Employments.Any())
                {
                    // 因為這裡獨立於下方的變數，我們直接使用已知的 UserID 與 InstitutionID 來建立關聯
                    context.Employments.AddRange(
                         new Employment { UserID = "U202607280002", InstitutionID = "I00001", OccupationID = "O09", Status = "Active", LicenseNumber = "VET-123456", StartDate = now.AddYears(-2) },
                         new Employment { UserID = "U202607280003", InstitutionID = "I00002", OccupationID = "O02", Status = "Active", LicenseNumber = "GROOM-654321", StartDate = now.AddYears(-1) }
                     );
                    context.SaveChanges();
                }


                // ==========================================
                // 原有的防呆機制：如果資料庫中已經有 UserProfile，
                // 代表下方的舊有測試資料都已經 Seed 過，直接 return
                // ==========================================
                if (context.UserProfiles.Any())
                {
                    return;
                }

                // ==========================================
                // 1. 建立 驗證方式 (UserCredential)
                // ==========================================
                var credLocal = new UserCredential { CredentialID = "C01", AuthProvider = "Local" };
                var credGoogle = new UserCredential { CredentialID = "C02", AuthProvider = "Google" };
                var credLine = new UserCredential { CredentialID = "C03", AuthProvider = "Line" };
                context.UserCredentials.AddRange(credLocal, credGoogle, credLine);
                context.SaveChanges();

                // ==========================================
                // 2. 建立 使用者基本資料 (UserProfile)
                // ==========================================
                var admin = new UserProfile { UserID = "U202607280001", Name = "系統管理員", Phone = "0900000000", Authorization = 9, Email = "admin@test.com", Status = 1, JoinDate = now };
                var vet = new UserProfile { UserID = "U202607280002", Name = "預設獸醫", Phone = "0911111111", Authorization = 2, Email = "vet@test.com", Status = 1, JoinDate = now };
                var groomer = new UserProfile { UserID = "U202607280003", Name = "陳美容", Phone = "0922222222", Authorization = 1, Email = "groomer@test.com", Status = 1, JoinDate = now };
                var owner = new UserProfile { UserID = "U202607280004", Name = "林飼主", Phone = "0933333333", Authorization = 0, Email = "owner@test.com", Status = 1, JoinDate = now };
                context.UserProfiles.AddRange(admin, vet, groomer, owner);
                context.SaveChanges();

                // ==========================================
                // 3. 建立 帳號密碼明細 (UserCredentialDetail)
                // ==========================================
                context.UserCredentialDetails.AddRange(
                    new UserCredentialDetail { UserID = admin.UserID, CredentialID = credLocal.CredentialID, Identifier = "admin", PasswordHash = "hashed_admin_pwd", CreatedDate = now },
                    new UserCredentialDetail { UserID = vet.UserID, CredentialID = credLocal.CredentialID, Identifier = vet.Email, PasswordHash = "hashed_vet_pwd", CreatedDate = now },
                    new UserCredentialDetail { UserID = owner.UserID, CredentialID = credGoogle.CredentialID, Identifier = "google_id_12345", CreatedDate = now }
                );
                context.SaveChanges();

                // ==========================================
                // 4.1 建立 機構 (InstitutionType)
                // ==========================================
                context.InstitutionTypes.AddRange(
                    new InstitutionType { TypeId = "IT01", Name = "動物醫院" },
                    new InstitutionType { TypeId = "IT02", Name = "寵物美容坊" }
                );
                context.SaveChanges();

                // ==========================================
                // 4.2 建立 機構 (InstitutionStatus)
                // ==========================================
                context.InstitutionStatuses.AddRange(
                    new InstitutionStatus { StatusId = "S01", StatusName = "啟用" },
                    new InstitutionStatus { StatusId = "S02", StatusName = "停用" }
                );
                context.SaveChanges();

                // ==========================================
                // 4. 建立 機構 (Institution)
                // ==========================================
                var clinic = new Institution { InstitutionID = "I00001", Name = "快樂動物醫院", TypeId = "IT01", Phone = "07-1234567", Address = "高雄市苓雅區某某路1號", StatusId = "S01" };
                var salon = new Institution { InstitutionID = "I00002", Name = "毛小孩美容坊", TypeId = "IT02", Phone = "07-7654321", Address = "高雄市鳳山區某某路2號", StatusId = "S01" };
                context.Institutions.AddRange(clinic, salon);
                context.SaveChanges();

                // ==========================================
                // 6. 建立 寵物資料 (Pet)
                // ==========================================
                var petDog = new Pet { OwnerID = owner.UserID, Name = "小白", Species = "Dog", Breed = "馬爾濟斯", Gender = "M", DateOfBirth = new DateTime(2021, 5, 10), SpecialNotes = "皮膚容易過敏" };
                var petCat = new Pet { OwnerID = owner.UserID, Name = "胖橘", Species = "Cat", Breed = "米克斯", Gender = "F", DateOfBirth = new DateTime(2022, 10, 1) };
                context.Pets.AddRange(petDog, petCat);
                context.SaveChanges();

                // ==========================================
                // 7. 建立 資料授權紀錄 (AuthorizationRecord)
                // ==========================================
                context.AuthorizationRecords.AddRange(
                    new AuthorizationRecord { DataAuthorizationNumber = "R20260803-I00001-01", PetID = petDog.PetID, InstitutionID = clinic.InstitutionID, GrantedDate = now.AddMonths(-6), Status = "Active" },
                    new AuthorizationRecord { DataAuthorizationNumber = "R20260803-I00002-01", PetID = petCat.PetID, InstitutionID = salon.InstitutionID, GrantedDate = now.AddMonths(-2), Status = "Active" }
                );

                // ==========================================
                // 8. 建立 預約紀錄 (Appointment)
                // ==========================================
                context.Appointments.AddRange(
                    new Appointment { AppointmentID = "A20260803-I00001-01", PetID = petDog.PetID, InstitutionID = clinic.InstitutionID, ServiceType = "健康檢查", ApptDateTime = now.AddDays(2), Status = "Confirmed" },
                    new Appointment { AppointmentID = "A20260803-I00002-01", PetID = petCat.PetID, InstitutionID = salon.InstitutionID, ServiceType = "洗澡修剪", ApptDateTime = now.AddDays(5), Status = "Pending" }
                );
                context.SaveChanges();

                // ==========================================
                // 9. 建立 醫療詞彙 (MedicalVocabularyDictionary)
                // ==========================================
                var vocab1 = new MedicalVocabularyDictionary { TermCode = "ICD10-A09", Category = "Disease", TermName = "腸胃炎", UpdatedAt = now };
                var vocab2 = new MedicalVocabularyDictionary { TermCode = "ICD10-R11", Category = "Symptom", TermName = "噁心與嘔吐", UpdatedAt = now };
                context.MedicalVocabularyDictionaries.AddRange(vocab1, vocab2);
                context.SaveChanges();

                // ==========================================
                // 10. 建立 病歷與處方 (MedicalRecord, RecordDiagnosis, Prescription)
                // ==========================================
                var record = new MedicalRecord { PetID = petDog.PetID, VetID = vet.UserID, VisitDate = now.AddDays(-10), Symptoms = "食慾不佳，連續嘔吐兩次", Diagnosis = "急性腸胃炎" };
                context.MedicalRecords.Add(record);
                context.SaveChanges();

                context.RecordDiagnoses.Add(new RecordDiagnosis { RecordID = record.RecordID, TermID = vocab1.TermID, Severity = "Moderate", Remark = "注意水分補充", CreatedDate = now });
                context.Prescriptions.Add(new Prescription { RecordID = record.RecordID, MedicationName = "Cerenia 止吐藥", Dosage = "16mg", Frequency = "一天一次", Instructions = "飯後餵食，若持續嘔吐須回診" });
                context.SaveChanges();

                // ==========================================
                // 11. 建立 美容日誌與照片 (CareGroomingLog, LogPhoto)
                // ==========================================
                var log = new CareGroomingLog { PetID = petCat.PetID, StaffID = groomer.UserID, LogDate = now.AddDays(-20), ActivityDetails = "完成洗澡、剪指甲、清耳朵。情緒稍稍緊張但無攻擊性。", DietNotes = "給予肉泥零食，進食正常" };
                context.CareGroomingLogs.Add(log);
                context.SaveChanges();

                context.LogPhotos.Add(new LogPhoto { LogID = log.LogID, ImageURL = "https://example.com/photos/cat_grooming_01.jpg" });
                context.SaveChanges();

                // ==========================================
                // 12. 建立 排班表 (TaskSchedule)
                // ==========================================
                context.TaskSchedules.Add(
                    new TaskSchedule { StaffID = groomer.UserID, InstitutionID = salon.InstitutionID, CreatedDate = now, StartTime = now.AddHours(1), EndTime = now.AddHours(5), TaskDescription = "下午場次美容服務排班", Status = "Pending" }
                );
                context.SaveChanges();
            }
        }
    }
}