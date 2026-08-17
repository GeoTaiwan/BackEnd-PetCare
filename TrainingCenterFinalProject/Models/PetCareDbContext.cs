using Microsoft.EntityFrameworkCore;

namespace TrainingCenterFinalProject.Models
{
    public class PetCareDbContext : DbContext
    {
        
        public PetCareDbContext()
        {
        }
        public PetCareDbContext(DbContextOptions<PetCareDbContext> options) : base(options)
        {
        }

        // ==========================================
        // 宣告 DbSets (對應資料庫中的 Tables)
        // ==========================================
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public virtual DbSet<UserCredentialDetail> UserCredentialDetails { get; set; } = null!;
        public virtual DbSet<UserCredential> UserCredentials { get; set; } = null!;
        public virtual DbSet<InstitutionType> InstitutionTypes { get; set; } = null!;
        public virtual DbSet<InstitutionStatus> InstitutionStatuses { get; set; } = null!;
        public virtual DbSet<Institution> Institutions { get; set; } = null!;
        public virtual DbSet<Employment> Employments { get; set; } = null!;
        public virtual DbSet<Pet> Pets { get; set; } = null!;
        public virtual DbSet<AuthorizationRecord> AuthorizationRecords { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<MedicalVocabularyDictionary> MedicalVocabularyDictionaries { get; set; } = null!;
        public virtual DbSet<RecordDiagnosis> RecordDiagnoses { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;
        public virtual DbSet<CareGroomingLog> CareGroomingLogs { get; set; } = null!;
        public virtual DbSet<LogPhoto> LogPhotos { get; set; } = null!;
        public virtual DbSet<TaskSchedule> TaskSchedules { get; set; } = null!;
        public virtual DbSet<SysMenu> SysMenus { get; set; } = null!;
        public DbSet<Occupations> Occupations { get; set; } = null!;
        public DbSet<PetWeight> PetWeight { get; set; } = null!;
        public DbSet<Role> Role { get; set; } = null!;
        // ==========================================
        // OnModelCreating - 進階設定與關聯定義
        // ==========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. 設定複合主鍵 (Composite Keys)
            // 雖然 Model 裡已經加了 [PrimaryKey]，但寫在這裡是保證相容舊版與最穩定的做法
            modelBuilder.Entity<UserCredentialDetail>()
                .HasKey(c => new { c.UserID, c.CredentialID });

            modelBuilder.Entity<Employment>()
                .HasKey(e => new { e.UserID, e.InstitutionID });

            // 2. 關閉預設的級聯刪除 (Cascade Delete)
            // 避免因為刪除一個機構 (Institution) 或使用者 (UserProfile)，就連帶把所有關聯的醫療紀錄、寵物都刪掉
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
                // 若您希望某個關聯可以被連帶刪除，可以在下方單獨覆寫
            }

            // --- 單獨開放特定實體的級聯刪除 (例如：刪除了病歷，明細跟著刪) ---

            // 刪除 MedicalRecord 時，連帶刪除 RecordDiagnosis
            modelBuilder.Entity<RecordDiagnosis>()
                .HasOne(rd => rd.MedicalRecord)
                .WithMany(m => m.RecordDiagnoses)
                .HasForeignKey(rd => rd.RecordID)
                .OnDelete(DeleteBehavior.Cascade);

            // 刪除 MedicalRecord 時，連帶刪除 Prescription
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.MedicalRecord)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.RecordID)
                .OnDelete(DeleteBehavior.Cascade);

            // 刪除 CareGroomingLog 時，連帶刪除 LogPhoto
            modelBuilder.Entity<LogPhoto>()
                .HasOne(lp => lp.CareGroomingLog)
                .WithMany(cgl => cgl.LogPhotos)
                .HasForeignKey(lp => lp.LogID)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. 設定特定欄位的預設值 (Default Values)
            // 雖然 Entity 已使用 [DatabaseGenerated]，但若要在 DB 層級產生 Constraint，可寫在這裡
            modelBuilder.Entity<UserProfile>()
                .Property(u => u.JoinDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserCredentialDetail>()
                .Property(uc => uc.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            // --- Seed Data 初始資料 ---
            modelBuilder.Entity<SysMenu>().HasData(
                new SysMenu { MenuId = 1, MenuName = "首頁", ControllerName = "Home", ActionName = "Index", SortOrder = 1, RequiredAuthLevel = 0, IsVisible = true },
                new SysMenu { MenuId = 2, MenuName = "使用者列表", ControllerName = "User", ActionName = "Index", SortOrder = 2, RequiredAuthLevel = 1, IsVisible = true },
                new SysMenu { MenuId = 3, MenuName = "寵物病歷管理", ControllerName = "MedicalRecord", ActionName = "Index", SortOrder = 3, RequiredAuthLevel = 2, IsVisible = true }
            );
        }
    }
}