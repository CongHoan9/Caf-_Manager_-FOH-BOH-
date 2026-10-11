/* ============================================================
   BANCOFFEE - SCRIPT TAO CO SO DU LIEU
   Khuon tham chieu: BanOto.sql (giu nguyen quy uoc sp_<bang>_<hanh dong>)
   Cach dung: chay toan bo file nay tren SQL Server de tao database [BanCoffee]
   ============================================================ */

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BanCoffee')
BEGIN
    CREATE DATABASE [BanCoffee];
END
GO

USE [BanCoffee]
GO

/* ============================================================
   1. TAO BANG
   ============================================================ */

-- Nhan vien / quan tri (dang nhap he thong)
IF OBJECT_ID(N'dbo.[user]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[user](
    [user_id] [varchar](50) NOT NULL,
    [hoten] [nvarchar](150) NULL,
    [ngaysinh] [date] NULL,
    [diachi] [nvarchar](250) NULL,
    [gioitinh] [nvarchar](30) NULL,
    [email] [varchar](150) NULL,
    [taikhoan] [varchar](30) NULL,
    [matkhau] [varchar](60) NULL,
    [role] [varchar](30) NULL,          -- Admin / Staff
    [image_url] [varchar](300) NULL,
    CONSTRAINT [PK_user] PRIMARY KEY CLUSTERED ([user_id] ASC)
) ON [PRIMARY]
END
GO

-- Khach hang dang ky (web/app)
IF OBJECT_ID(N'dbo.customer', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[customer](
    [customer_email] [varchar](100) NOT NULL,
    [customer_password] [varchar](150) NULL,
    CONSTRAINT [PK_customer] PRIMARY KEY CLUSTERED ([customer_email] ASC)
) ON [PRIMARY]
END
GO

-- Nhom do uong (menu phan cap)
IF OBJECT_ID(N'dbo.item_group', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[item_group](
    [parent_item_group_id] [varchar](50) NULL,
    [item_group_id] [varchar](50) NOT NULL,
    [item_group_name] [nvarchar](250) NULL,
    [seq_num] [smallint] NULL,
    [url] [varchar](150) NULL,
    CONSTRAINT [PK_item_group] PRIMARY KEY CLUSTERED ([item_group_id] ASC)
) ON [PRIMARY]
END
GO

-- Mon uong
IF OBJECT_ID(N'dbo.item', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[item](
    [item_id] [varchar](50) NOT NULL,
    [item_group_id] [varchar](50) NULL,
    [item_name] [nvarchar](150) NULL,
    [item_image] [varchar](250) NULL,
    [item_price] [float] NULL,
    CONSTRAINT [PK_item] PRIMARY KEY CLUSTERED ([item_id] ASC)
) ON [PRIMARY]
END
GO

-- Khuyen mai (truoc hoa don de xai FK)
IF OBJECT_ID(N'dbo.khuyen_mai', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[khuyen_mai](
    [khuyen_mai_id] [varchar](50) NOT NULL,
    [ma_khuyen_mai] [varchar](30) NULL UNIQUE,
    [ten_khuyen_mai] [nvarchar](150) NULL,
    [loai_giam] [nvarchar](30) NULL,
    [gia_tri_giam] [float] NULL,
    [ngay_bat_dau] [datetime] NULL,
    [ngay_ket_thuc] [datetime] NULL,
    [trang_thai] [nvarchar](30) NULL,
    CONSTRAINT [PK_khuyen_mai] PRIMARY KEY CLUSTERED ([khuyen_mai_id] ASC)
) ON [PRIMARY]
END
GO

-- Ban (truoc hoa don de xai FK)
IF OBJECT_ID(N'dbo.ban', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ban](
    [ban_id] [varchar](50) NOT NULL,
    [ten_ban] [nvarchar](100) NULL,
    [khu_vuc] [nvarchar](100) NULL,
    [so_cho] [int] NULL,
    [trang_thai] [nvarchar](30) NULL,
    CONSTRAINT [PK_ban] PRIMARY KEY CLUSTERED ([ban_id] ASC)
) ON [PRIMARY]
END
GO

-- Hoa don (don hang) 
IF OBJECT_ID(N'dbo.hoa_don', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[hoa_don](
    [ma_hoa_don] [varchar](50) NOT NULL,
    [ho_ten] [nvarchar](150) NULL,
    [dia_chi] [nvarchar](250) NULL,
    [ngay_tao] [datetime] NULL CONSTRAINT [DF_hoa_don_ngay_tao] DEFAULT (GETDATE()),
    [tong_tien] [float] NULL,
    [giam_gia] [float] NULL,
    [thanh_tien] [float] NULL,
    [hinh_thuc_thanh_toan] [nvarchar](50) NULL,   -- Tien mat / Chuyen khoan / Vi dien tu
    [ghi_chu] [nvarchar](250) NULL,
    [user_id] [varchar](50) NULL,                -- nhan vien thanh toan
    [ban_id] [varchar](50) NULL,                 -- ban phuc vu
    [khuyen_mai_id] [varchar](50) NULL,          -- ma KM ap dung
    CONSTRAINT [PK_hoa_don] PRIMARY KEY CLUSTERED ([ma_hoa_don] ASC)
) ON [PRIMARY]
END
GO

-- Chi tiet hoa don
IF OBJECT_ID(N'dbo.chi_tiet_hoa_don', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[chi_tiet_hoa_don](
    [ma_chi_tiet] [varchar](50) NOT NULL,
    [ma_hoa_don] [varchar](50) NULL,
    [item_id] [varchar](50) NULL,
    [so_luong] [int] NULL,
    [don_gia] [float] NULL,
    CONSTRAINT [PK_chi_tiet_hoa_don] PRIMARY KEY CLUSTERED ([ma_chi_tiet] ASC)
) ON [PRIMARY]
END
GO

-- Tin tuc - khuyen mai
IF OBJECT_ID(N'dbo.news', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[news](
    [news_id] [varchar](50) NOT NULL,
    [title] [nvarchar](150) NULL,
    [content_news] [nvarchar](max) NULL,
    [created_date] [datetime] NULL,
    [user_id] [varchar](50) NULL,
    CONSTRAINT [PK_news] PRIMARY KEY CLUSTERED ([news_id] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- Nguyen lieu
IF OBJECT_ID(N'dbo.nguyen_lieu', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[nguyen_lieu](
    [nguyen_lieu_id] [varchar](50) NOT NULL,
    [ten_nguyen_lieu] [nvarchar](150) NULL,
    [don_vi] [nvarchar](30) NULL,
    [so_luong_ton] [float] NULL,
    [gia_nhap] [float] NULL,
    [ngay_nhap_cuoi] [datetime] NULL,
    CONSTRAINT [PK_nguyen_lieu] PRIMARY KEY CLUSTERED ([nguyen_lieu_id] ASC)
) ON [PRIMARY]
END
GO

-- Ca lam
IF OBJECT_ID(N'dbo.ca_lam', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ca_lam](
    [ca_lam_id] [varchar](50) NOT NULL,
    [ten_ca] [nvarchar](100) NULL,
    [gio_bat_dau] [time](7) NULL,
    [gio_ket_thuc] [time](7) NULL,
    CONSTRAINT [PK_ca_lam] PRIMARY KEY CLUSTERED ([ca_lam_id] ASC)
) ON [PRIMARY]
END
GO

-- Lich lam viec
IF OBJECT_ID(N'dbo.lich_lam_viec', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[lich_lam_viec](
    [lich_id] [varchar](50) NOT NULL,
    [user_id] [varchar](50) NULL,
    [ca_lam_id] [varchar](50) NULL,
    [ngay_lam] [date] NULL,
    [ghi_chu] [nvarchar](250) NULL,
    CONSTRAINT [PK_lich_lam_viec] PRIMARY KEY CLUSTERED ([lich_id] ASC)
) ON [PRIMARY]
END
GO


/* ============================================================
   2. KHOA NGOAI
   ============================================================ */

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_chi_tiet_hoa_don_hoa_don')
    ALTER TABLE [dbo].[chi_tiet_hoa_don] WITH CHECK ADD CONSTRAINT [FK_chi_tiet_hoa_don_hoa_don]
    FOREIGN KEY([ma_hoa_don]) REFERENCES [dbo].[hoa_don] ([ma_hoa_don])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_chi_tiet_hoa_don_item')
    ALTER TABLE [dbo].[chi_tiet_hoa_don] WITH CHECK ADD CONSTRAINT [FK_chi_tiet_hoa_don_item]
    FOREIGN KEY([item_id]) REFERENCES [dbo].[item] ([item_id])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_item_item_group1')
    ALTER TABLE [dbo].[item] WITH CHECK ADD CONSTRAINT [FK_item_item_group1]
    FOREIGN KEY([item_group_id]) REFERENCES [dbo].[item_group] ([item_group_id])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_hoa_don_user')
    ALTER TABLE [dbo].[hoa_don] WITH CHECK ADD CONSTRAINT [FK_hoa_don_user]
    FOREIGN KEY([user_id]) REFERENCES [dbo].[user] ([user_id])
GO

-- Them khoa ngoai moi
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_hoa_don_ban')
    ALTER TABLE [dbo].[hoa_don] WITH CHECK ADD CONSTRAINT [FK_hoa_don_ban]
    FOREIGN KEY([ban_id]) REFERENCES [dbo].[ban] ([ban_id])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_hoa_don_khuyen_mai')
    ALTER TABLE [dbo].[hoa_don] WITH CHECK ADD CONSTRAINT [FK_hoa_don_khuyen_mai]
    FOREIGN KEY([khuyen_mai_id]) REFERENCES [dbo].[khuyen_mai] ([khuyen_mai_id])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_lich_lam_viec_user')
    ALTER TABLE [dbo].[lich_lam_viec] WITH CHECK ADD CONSTRAINT [FK_lich_lam_viec_user]
    FOREIGN KEY([user_id]) REFERENCES [dbo].[user] ([user_id])
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_lich_lam_viec_ca_lam')
    ALTER TABLE [dbo].[lich_lam_viec] WITH CHECK ADD CONSTRAINT [FK_lich_lam_viec_ca_lam]
    FOREIGN KEY([ca_lam_id]) REFERENCES [dbo].[ca_lam] ([ca_lam_id])
GO

/* ============================================================
   3. STORED PROCEDURE - USER
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_user_create]
(@user_id     varchar(50),
 @hoten       nvarchar(150),
 @ngaysinh    date,
 @diachi      nvarchar(250),
 @gioitinh    nvarchar(30),
 @email       varchar(150),
 @taikhoan    varchar(30),
 @matkhau     varchar(60),
 @role        varchar(30),
 @image_url   varchar(300)
)
AS
BEGIN
    INSERT INTO [user] ([user_id], [hoten], [ngaysinh], [diachi], [gioitinh], [email], [taikhoan], [matkhau], [role], [image_url])
    VALUES (@user_id, @hoten, @ngaysinh, @diachi, @gioitinh, @email, @taikhoan, @matkhau, @role, @image_url);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_user_update]
(@user_id     varchar(50),
 @hoten       nvarchar(150),
 @ngaysinh    date,
 @diachi      nvarchar(250),
 @gioitinh    nvarchar(30),
 @email       varchar(150),
 @taikhoan    varchar(30),
 @matkhau     varchar(60),
 @role        varchar(30),
 @image_url   varchar(300)
)
AS
BEGIN
    UPDATE [user] SET
        hoten     = @hoten,
        taikhoan  = @taikhoan,
        ngaysinh  = @ngaysinh,
        diachi    = @diachi,
        gioitinh  = @gioitinh,
        email     = @email,
        matkhau   = @matkhau,
        role      = @role,
        image_url = @image_url
    WHERE user_id = @user_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_user_delete]
(@user_id varchar(50))
AS
BEGIN
    DELETE FROM [user] WHERE user_id = @user_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_user_get_by_id]
(@user_id VARCHAR(50))
AS
BEGIN
    SELECT [user_id], [hoten], [ngaysinh], [diachi], [gioitinh], [email], [taikhoan], [matkhau], [role], [image_url]
    FROM [user]
    WHERE [user_id] = @user_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_user_get_by_username_password]
(@taikhoan varchar(30), @matkhau varchar(60))
AS
BEGIN
    SELECT [user_id], [hoten], [ngaysinh], [diachi], [gioitinh], [email], [taikhoan], [role], [image_url]
    FROM [user]
    WHERE taikhoan = @taikhoan AND matkhau = @matkhau;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_user_search]
(@page_index INT,
 @page_size  INT,
 @hoten      nvarchar(150),
 @taikhoan   varchar(30)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY u.hoten ASC)) AS RowNumber,
               u.user_id, u.hoten, u.ngaysinh, u.diachi, u.gioitinh, u.email, u.taikhoan, u.matkhau, u.role, u.image_url
        INTO #Results1
        FROM [user] AS u
        WHERE (u.taikhoan <> 'Admin')
          AND ((@hoten = '') OR (u.hoten LIKE N'%' + @hoten + N'%'))
          AND ((@taikhoan = '') OR (u.taikhoan = @taikhoan));
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY u.hoten ASC)) AS RowNumber,
               u.user_id, u.hoten, u.ngaysinh, u.diachi, u.gioitinh, u.email, u.taikhoan, u.matkhau, u.role, u.image_url
        INTO #Results2
        FROM [user] AS u
        WHERE (u.taikhoan <> 'Admin')
          AND ((@hoten = '') OR (u.hoten LIKE N'%' + @hoten + N'%'))
          AND ((@taikhoan = '') OR (u.taikhoan = @taikhoan));
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

-- New SP: sp_user_all
CREATE OR ALTER PROCEDURE [dbo].[sp_user_all]
AS
BEGIN
    SELECT [user_id], [hoten], [ngaysinh], [diachi], [gioitinh], [email], [taikhoan], [role], [image_url]
    FROM [user];
END
GO

-- New SP: sp_user_change_password
CREATE OR ALTER PROCEDURE [dbo].[sp_user_change_password]
(@user_id varchar(50),
 @old_password varchar(60),
 @new_password varchar(60)
)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [user] WHERE user_id = @user_id AND matkhau = @old_password)
    BEGIN
        UPDATE [user] SET matkhau = @new_password WHERE user_id = @user_id;
        SELECT 'Success' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Incorrect old password' AS Result;
    END
END
GO

-- New SP: sp_user_check_exists
CREATE OR ALTER PROCEDURE [dbo].[sp_user_check_exists]
(@taikhoan varchar(30))
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [user] WHERE taikhoan = @taikhoan)
        SELECT 1 AS IsExists;
    ELSE
        SELECT 0 AS IsExists;
END
GO


/* ============================================================
   4. STORED PROCEDURE - CUSTOMER
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_customer_create]
(@customer_email     VARCHAR(100),
 @customer_password  VARCHAR(150)
)
AS
BEGIN
    INSERT INTO customer (customer_email, customer_password)
    VALUES (@customer_email, @customer_password);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_customer_get_by_username_password]
(@customer_email    VARCHAR(100),
 @customer_password VARCHAR(150)
)
AS
BEGIN
    SELECT customer_email
    FROM customer
    WHERE customer_email = @customer_email AND customer_password = @customer_password;
END
GO


/* ============================================================
   5. STORED PROCEDURE - ITEM GROUP
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_item_group_get_data]
AS
BEGIN
    SELECT item_group.parent_item_group_id,
           item_group.item_group_id,
           item_group.item_group_name,
           item_group.seq_num,
           item_group.url
    FROM item_group
    ORDER BY item_group.seq_num;
END
GO

/* ============================================================
   6. STORED PROCEDURE - ITEM
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_item_all]
AS
BEGIN
    SELECT item.item_id, item.item_group_id, item.item_image, item.item_name, item.item_price
    FROM item;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_item_create]
(@item_id        VARCHAR(50),
 @item_group_id  VARCHAR(50),
 @item_image     VARCHAR(250),
 @item_name      NVARCHAR(150),
 @item_price     float
)
AS
BEGIN
    INSERT INTO item (item_id, item_group_id, item_image, item_name, item_price)
    VALUES (@item_id, @item_group_id, @item_image, @item_name, @item_price);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_item_update]
(@item_id        VARCHAR(50),
 @item_group_id  VARCHAR(50),
 @item_image     VARCHAR(250),
 @item_name      NVARCHAR(150),
 @item_price     float
)
AS
BEGIN
    UPDATE item SET
        item_group_id = @item_group_id,
        item_image    = @item_image,
        item_name     = @item_name,
        item_price    = @item_price
    WHERE item_id = @item_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_item_delete]
(@item_id varchar(50))
AS
BEGIN
    DELETE FROM item WHERE item_id = @item_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_item_get_by_id]
(@item_id VARCHAR(50))
AS
BEGIN
    SELECT item.item_id, item.item_group_id, item.item_image, item.item_name, item.item_price
    FROM item
    WHERE item.item_id = @item_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_item_search]
(@page_index    INT,
 @page_size     INT,
 @item_group_id Nvarchar(50),
 @item_name     Nvarchar(150)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY item_name ASC)) AS RowNumber,
               i.item_id, i.item_group_id, i.item_name, i.item_image, i.item_price
        INTO #Results1
        FROM [item] AS i
        WHERE (@item_name = '' Or i.item_name LIKE N'%' + @item_name + N'%')
          AND (@item_group_id = '' OR i.item_group_id = @item_group_id);
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY item_name ASC)) AS RowNumber,
               i.item_id, i.item_group_id, i.item_name, i.item_image, i.item_price
        INTO #Results2
        FROM [item] AS i
        WHERE (@item_name = '' Or i.item_name LIKE N'%' + @item_name + N'%')
          AND (@item_group_id = '' OR i.item_group_id = @item_group_id);
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

/* ============================================================
   7. STORED PROCEDURE - HOA DON (master - detail, OPENJSON)
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_hoa_don_create]
(@ma_hoa_don        VARCHAR(50),
 @ho_ten            NVARCHAR(150),
 @dia_chi           NVARCHAR(250),
 @ngay_tao          datetime,
 @tong_tien         float,
 @giam_gia          float,
 @thanh_tien        float,
 @hinh_thuc_thanh_toan nvarchar(50),
 @ghi_chu           NVARCHAR(250),
 @user_id           VARCHAR(50),
 @ban_id            VARCHAR(50) = NULL,
 @khuyen_mai_id     VARCHAR(50) = NULL,
 @listjson_chitiet  NVARCHAR(MAX)
)
AS
BEGIN
    INSERT INTO hoa_don (ma_hoa_don, ho_ten, dia_chi, ngay_tao, tong_tien, giam_gia, thanh_tien, hinh_thuc_thanh_toan, ghi_chu, user_id, ban_id, khuyen_mai_id)
    VALUES (@ma_hoa_don, @ho_ten, @dia_chi, ISNULL(@ngay_tao, GETDATE()), @tong_tien, @giam_gia, @thanh_tien, @hinh_thuc_thanh_toan, @ghi_chu, @user_id, @ban_id, @khuyen_mai_id);

    IF (@listjson_chitiet IS NOT NULL)
    BEGIN
        INSERT INTO chi_tiet_hoa_don (ma_chi_tiet, ma_hoa_don, item_id, so_luong, don_gia)
        SELECT JSON_VALUE(p.value, '$.ma_chi_tiet'),
               @ma_hoa_don,
               JSON_VALUE(p.value, '$.item_id'),
               JSON_VALUE(p.value, '$.so_luong'),
               JSON_VALUE(p.value, '$.don_gia')
        FROM OPENJSON(@listjson_chitiet) AS p;
    END;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_hoa_don_update]
(@ma_hoa_don        VARCHAR(50),
 @ho_ten            NVARCHAR(150),
 @dia_chi           NVARCHAR(250),
 @ngay_tao          datetime,
 @tong_tien         float,
 @giam_gia          float,
 @thanh_tien        float,
 @hinh_thuc_thanh_toan nvarchar(50),
 @ghi_chu           NVARCHAR(250),
 @user_id           VARCHAR(50),
 @ban_id            VARCHAR(50) = NULL,
 @khuyen_mai_id     VARCHAR(50) = NULL,
 @listjson_chitiet  NVARCHAR(MAX)
)
AS
BEGIN
    UPDATE hoa_don SET
        ho_ten = @ho_ten,
        dia_chi = @dia_chi,
        ngay_tao = @ngay_tao,
        tong_tien = @tong_tien,
        giam_gia = @giam_gia,
        thanh_tien = @thanh_tien,
        hinh_thuc_thanh_toan = @hinh_thuc_thanh_toan,
        ghi_chu = @ghi_chu,
        user_id = @user_id,
        ban_id = @ban_id,
        khuyen_mai_id = @khuyen_mai_id
    WHERE ma_hoa_don = @ma_hoa_don;

    IF (@listjson_chitiet IS NOT NULL)
    BEGIN
        -- Insert data to temp table
        SELECT
            JSON_VALUE(p.value, '$.ma_chi_tiet') AS ma_chi_tiet,
            JSON_VALUE(p.value, '$.ma_hoa_don') AS ma_hoa_don,
            JSON_VALUE(p.value, '$.item_id')    AS item_id,
            JSON_VALUE(p.value, '$.so_luong')   AS so_luong,
            JSON_VALUE(p.value, '$.don_gia')    AS don_gia,
            JSON_VALUE(p.value, '$.status')     AS status
        INTO #Results
        FROM OPENJSON(@listjson_chitiet) AS p;

        -- Insert data with STATUS = 1 (dong moi them)
        INSERT INTO chi_tiet_hoa_don (ma_chi_tiet, ma_hoa_don, item_id, so_luong, don_gia)
        SELECT #Results.ma_chi_tiet, @ma_hoa_don, #Results.item_id, #Results.so_luong, #Results.don_gia
        FROM #Results
        WHERE #Results.status = '1';

        -- Update data with STATUS = 2 (dong sua)
        UPDATE chi_tiet_hoa_don
        SET so_luong = #Results.so_luong,
            don_gia  = #Results.don_gia
        FROM #Results
        WHERE chi_tiet_hoa_don.ma_chi_tiet = #Results.ma_chi_tiet AND #Results.status = '2';

        -- Delete data with STATUS = 3 (dong xoa)
        DELETE C
        FROM chi_tiet_hoa_don C
        INNER JOIN #Results R ON C.ma_chi_tiet = R.ma_chi_tiet
        WHERE R.status = '3';

        DROP TABLE #Results;
    END;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_hoa_don_delete]
(@ma_hoa_don varchar(50))
AS
BEGIN
    DELETE FROM [chi_tiet_hoa_don] WHERE ma_hoa_don = @ma_hoa_don;
    DELETE FROM [hoa_don] WHERE ma_hoa_don = @ma_hoa_don;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_hoa_don_get_by_id]
(@ma_hoa_don VARCHAR(50))
AS
BEGIN
    SELECT h.ma_hoa_don,
           h.ho_ten,
           h.dia_chi,
           h.ngay_tao,
           h.tong_tien,
           h.giam_gia,
           h.thanh_tien,
           h.hinh_thuc_thanh_toan,
           h.ghi_chu,
           h.user_id,
           h.ban_id,
           h.khuyen_mai_id,
           (
               SELECT c.ma_chi_tiet,
                      c.ma_hoa_don,
                      c.item_id,
                      c.so_luong,
                      c.don_gia
               FROM chi_tiet_hoa_don AS c
               WHERE c.ma_hoa_don = h.ma_hoa_don FOR JSON PATH
           ) AS listjson_chitiet
    FROM hoa_don h
    WHERE h.ma_hoa_don = @ma_hoa_don;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_hoa_don_search]
(@page_index INT,
 @page_size  INT,
 @hoten      Nvarchar(150),
 @diachi     Nvarchar(250)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY h.ngay_tao DESC, ho_ten ASC)) AS RowNumber,
               h.ma_hoa_don, h.ho_ten, h.dia_chi, h.ngay_tao, h.tong_tien, h.giam_gia, h.thanh_tien,
               h.hinh_thuc_thanh_toan, h.ghi_chu, h.user_id, h.ban_id, h.khuyen_mai_id
        INTO #Results1
        FROM [hoa_don] AS h
        WHERE (@hoten = '' Or h.ho_ten LIKE N'%' + @hoten + N'%')
          AND (@diachi = '' Or h.dia_chi LIKE N'%' + @diachi + N'%');
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY h.ngay_tao DESC, ho_ten ASC)) AS RowNumber,
               h.ma_hoa_don, h.ho_ten, h.dia_chi, h.ngay_tao, h.tong_tien, h.giam_gia, h.thanh_tien,
               h.hinh_thuc_thanh_toan, h.ghi_chu, h.user_id, h.ban_id, h.khuyen_mai_id
        INTO #Results2
        FROM [hoa_don] AS h
        WHERE (@hoten = '' Or h.ho_ten LIKE N'%' + @hoten + N'%')
          AND (@diachi = '' Or h.dia_chi LIKE N'%' + @diachi + N'%');
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO


/* ============================================================
   8. STORED PROCEDURE - NEWS
   ============================================================ */

CREATE OR ALTER PROCEDURE [dbo].[sp_news_create]
(@news_id      varchar(50),
 @title        nvarchar(150),
 @content_news nvarchar(max)
)
AS
BEGIN
    INSERT INTO [news] (news_id, title, content_news, created_date)
    VALUES (@news_id, @title, @content_news, GETDATE());
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_news_all]
AS
BEGIN
    SELECT news.news_id, news.title, news.content_news, news.created_date
    FROM news
    ORDER BY news.created_date DESC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_news_get_by_id]
(@news_id VARCHAR(50))
AS
BEGIN
    SELECT news.news_id, news.title, news.content_news, news.created_date
    FROM news
    WHERE news.news_id = @news_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_news_delete]
(@news_id varchar(50))
AS
BEGIN
    DELETE FROM [news] WHERE news_id = @news_id;
    SELECT '';
END
GO

/* ============================================================
   9. STORED PROCEDURE - BAN (NEW)
   ============================================================ */
CREATE OR ALTER PROCEDURE [dbo].[sp_ban_create]
(@ban_id      varchar(50),
 @ten_ban     nvarchar(100),
 @khu_vuc     nvarchar(100),
 @so_cho      int,
 @trang_thai  nvarchar(30)
)
AS
BEGIN
    INSERT INTO [ban] (ban_id, ten_ban, khu_vuc, so_cho, trang_thai)
    VALUES (@ban_id, @ten_ban, @khu_vuc, @so_cho, @trang_thai);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_update]
(@ban_id      varchar(50),
 @ten_ban     nvarchar(100),
 @khu_vuc     nvarchar(100),
 @so_cho      int,
 @trang_thai  nvarchar(30)
)
AS
BEGIN
    UPDATE [ban] SET
        ten_ban = @ten_ban,
        khu_vuc = @khu_vuc,
        so_cho = @so_cho,
        trang_thai = @trang_thai
    WHERE ban_id = @ban_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_delete]
(@ban_id varchar(50))
AS
BEGIN
    DELETE FROM [ban] WHERE ban_id = @ban_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_get_by_id]
(@ban_id varchar(50))
AS
BEGIN
    SELECT ban_id, ten_ban, khu_vuc, so_cho, trang_thai
    FROM [ban]
    WHERE ban_id = @ban_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_all]
AS
BEGIN
    SELECT ban_id, ten_ban, khu_vuc, so_cho, trang_thai
    FROM [ban]
    ORDER BY khu_vuc ASC, ten_ban ASC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_update_trang_thai]
(@ban_id varchar(50), @trang_thai nvarchar(30))
AS
BEGIN
    UPDATE [ban] SET trang_thai = @trang_thai WHERE ban_id = @ban_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ban_search]
(@page_index INT,
 @page_size  INT,
 @ten_ban    nvarchar(100),
 @khu_vuc    nvarchar(100),
 @trang_thai nvarchar(30)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY khu_vuc ASC, ten_ban ASC)) AS RowNumber,
               ban_id, ten_ban, khu_vuc, so_cho, trang_thai
        INTO #Results1
        FROM [ban]
        WHERE (@ten_ban = '' OR ten_ban LIKE N'%' + @ten_ban + N'%')
          AND (@khu_vuc = '' OR khu_vuc LIKE N'%' + @khu_vuc + N'%')
          AND (@trang_thai = '' OR trang_thai = @trang_thai);
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY khu_vuc ASC, ten_ban ASC)) AS RowNumber,
               ban_id, ten_ban, khu_vuc, so_cho, trang_thai
        INTO #Results2
        FROM [ban]
        WHERE (@ten_ban = '' OR ten_ban LIKE N'%' + @ten_ban + N'%')
          AND (@khu_vuc = '' OR khu_vuc LIKE N'%' + @khu_vuc + N'%')
          AND (@trang_thai = '' OR trang_thai = @trang_thai);
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

/* ============================================================
   10. STORED PROCEDURE - NGUYEN LIEU (NEW)
   ============================================================ */
CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_create]
(@nguyen_lieu_id   varchar(50),
 @ten_nguyen_lieu  nvarchar(150),
 @don_vi           nvarchar(30),
 @so_luong_ton     float,
 @gia_nhap         float,
 @ngay_nhap_cuoi   datetime
)
AS
BEGIN
    INSERT INTO [nguyen_lieu] (nguyen_lieu_id, ten_nguyen_lieu, don_vi, so_luong_ton, gia_nhap, ngay_nhap_cuoi)
    VALUES (@nguyen_lieu_id, @ten_nguyen_lieu, @don_vi, @so_luong_ton, @gia_nhap, @ngay_nhap_cuoi);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_update]
(@nguyen_lieu_id   varchar(50),
 @ten_nguyen_lieu  nvarchar(150),
 @don_vi           nvarchar(30),
 @so_luong_ton     float,
 @gia_nhap         float,
 @ngay_nhap_cuoi   datetime
)
AS
BEGIN
    UPDATE [nguyen_lieu] SET
        ten_nguyen_lieu = @ten_nguyen_lieu,
        don_vi = @don_vi,
        so_luong_ton = @so_luong_ton,
        gia_nhap = @gia_nhap,
        ngay_nhap_cuoi = @ngay_nhap_cuoi
    WHERE nguyen_lieu_id = @nguyen_lieu_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_delete]
(@nguyen_lieu_id varchar(50))
AS
BEGIN
    DELETE FROM [nguyen_lieu] WHERE nguyen_lieu_id = @nguyen_lieu_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_get_by_id]
(@nguyen_lieu_id varchar(50))
AS
BEGIN
    SELECT nguyen_lieu_id, ten_nguyen_lieu, don_vi, so_luong_ton, gia_nhap, ngay_nhap_cuoi
    FROM [nguyen_lieu]
    WHERE nguyen_lieu_id = @nguyen_lieu_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_all]
AS
BEGIN
    SELECT nguyen_lieu_id, ten_nguyen_lieu, don_vi, so_luong_ton, gia_nhap, ngay_nhap_cuoi
    FROM [nguyen_lieu]
    ORDER BY ten_nguyen_lieu ASC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_search]
(@page_index      INT,
 @page_size       INT,
 @ten_nguyen_lieu nvarchar(150)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY ten_nguyen_lieu ASC)) AS RowNumber,
               nguyen_lieu_id, ten_nguyen_lieu, don_vi, so_luong_ton, gia_nhap, ngay_nhap_cuoi
        INTO #Results1
        FROM [nguyen_lieu]
        WHERE (@ten_nguyen_lieu = '' OR ten_nguyen_lieu LIKE N'%' + @ten_nguyen_lieu + N'%');
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY ten_nguyen_lieu ASC)) AS RowNumber,
               nguyen_lieu_id, ten_nguyen_lieu, don_vi, so_luong_ton, gia_nhap, ngay_nhap_cuoi
        INTO #Results2
        FROM [nguyen_lieu]
        WHERE (@ten_nguyen_lieu = '' OR ten_nguyen_lieu LIKE N'%' + @ten_nguyen_lieu + N'%');
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_nguyen_lieu_nhap_them]
(@nguyen_lieu_id varchar(50),
 @so_luong_them  float,
 @gia_nhap_moi   float,
 @ngay_nhap      datetime
)
AS
BEGIN
    UPDATE [nguyen_lieu] 
    SET so_luong_ton = so_luong_ton + @so_luong_them,
        gia_nhap = @gia_nhap_moi,
        ngay_nhap_cuoi = @ngay_nhap
    WHERE nguyen_lieu_id = @nguyen_lieu_id;
    SELECT '';
END
GO

/* ============================================================
   11. STORED PROCEDURE - CA LAM (NEW)
   ============================================================ */
CREATE OR ALTER PROCEDURE [dbo].[sp_ca_lam_create]
(@ca_lam_id   varchar(50),
 @ten_ca      nvarchar(100),
 @gio_bat_dau time(7),
 @gio_ket_thuc time(7)
)
AS
BEGIN
    INSERT INTO [ca_lam] (ca_lam_id, ten_ca, gio_bat_dau, gio_ket_thuc)
    VALUES (@ca_lam_id, @ten_ca, @gio_bat_dau, @gio_ket_thuc);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ca_lam_update]
(@ca_lam_id   varchar(50),
 @ten_ca      nvarchar(100),
 @gio_bat_dau time(7),
 @gio_ket_thuc time(7)
)
AS
BEGIN
    UPDATE [ca_lam] SET
        ten_ca = @ten_ca,
        gio_bat_dau = @gio_bat_dau,
        gio_ket_thuc = @gio_ket_thuc
    WHERE ca_lam_id = @ca_lam_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ca_lam_delete]
(@ca_lam_id varchar(50))
AS
BEGIN
    DELETE FROM [ca_lam] WHERE ca_lam_id = @ca_lam_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ca_lam_get_by_id]
(@ca_lam_id varchar(50))
AS
BEGIN
    SELECT ca_lam_id, ten_ca, gio_bat_dau, gio_ket_thuc
    FROM [ca_lam]
    WHERE ca_lam_id = @ca_lam_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ca_lam_all]
AS
BEGIN
    SELECT ca_lam_id, ten_ca, gio_bat_dau, gio_ket_thuc
    FROM [ca_lam]
    ORDER BY gio_bat_dau ASC;
END
GO

/* ============================================================
   12. STORED PROCEDURE - LICH LAM VIEC (NEW)
   ============================================================ */
CREATE OR ALTER PROCEDURE [dbo].[sp_lich_lam_viec_create]
(@lich_id    varchar(50),
 @user_id    varchar(50),
 @ca_lam_id  varchar(50),
 @ngay_lam   date,
 @ghi_chu    nvarchar(250)
)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [lich_lam_viec] WHERE user_id = @user_id AND ngay_lam = @ngay_lam AND ca_lam_id = @ca_lam_id)
    BEGIN
        RAISERROR(N'NhÃ¢n viÃªn nÃ y Ä‘Ã£ Ä‘Æ°á»£c phÃ¢n ca lÃ m nÃ y trong ngÃ y hÃ´m nay.', 16, 1);
        RETURN;
    END

    INSERT INTO [lich_lam_viec] (lich_id, user_id, ca_lam_id, ngay_lam, ghi_chu)
    VALUES (@lich_id, @user_id, @ca_lam_id, @ngay_lam, @ghi_chu);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_lich_lam_viec_update]
(@lich_id    varchar(50),
 @user_id    varchar(50),
 @ca_lam_id  varchar(50),
 @ngay_lam   date,
 @ghi_chu    nvarchar(250)
)
AS
BEGIN
    UPDATE [lich_lam_viec] SET
        user_id = @user_id,
        ca_lam_id = @ca_lam_id,
        ngay_lam = @ngay_lam,
        ghi_chu = @ghi_chu
    WHERE lich_id = @lich_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_lich_lam_viec_delete]
(@lich_id varchar(50))
AS
BEGIN
    DELETE FROM [lich_lam_viec] WHERE lich_id = @lich_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_lich_lam_viec_get_by_id]
(@lich_id varchar(50))
AS
BEGIN
    SELECT l.lich_id, l.user_id, l.ca_lam_id, l.ngay_lam, l.ghi_chu,
           u.hoten,
           c.ten_ca
    FROM [lich_lam_viec] l
    LEFT JOIN [user] u ON l.user_id = u.user_id
    LEFT JOIN [ca_lam] c ON l.ca_lam_id = c.ca_lam_id
    WHERE l.lich_id = @lich_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_lich_lam_viec_search]
(@page_index INT,
 @page_size  INT,
 @user_id    varchar(50),
 @ngay_lam   date,
 @ca_lam_id  varchar(50)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY l.ngay_lam DESC)) AS RowNumber,
               l.lich_id, l.user_id, l.ca_lam_id, l.ngay_lam, l.ghi_chu,
               u.hoten,
               c.ten_ca
        INTO #Results1
        FROM [lich_lam_viec] l
        LEFT JOIN [user] u ON l.user_id = u.user_id
        LEFT JOIN [ca_lam] c ON l.ca_lam_id = c.ca_lam_id
        WHERE (@user_id = '' OR l.user_id = @user_id)
          AND (@ngay_lam IS NULL OR l.ngay_lam = @ngay_lam)
          AND (@ca_lam_id = '' OR l.ca_lam_id = @ca_lam_id);
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY l.ngay_lam DESC)) AS RowNumber,
               l.lich_id, l.user_id, l.ca_lam_id, l.ngay_lam, l.ghi_chu,
               u.hoten,
               c.ten_ca
        INTO #Results2
        FROM [lich_lam_viec] l
        LEFT JOIN [user] u ON l.user_id = u.user_id
        LEFT JOIN [ca_lam] c ON l.ca_lam_id = c.ca_lam_id
        WHERE (@user_id = '' OR l.user_id = @user_id)
          AND (@ngay_lam IS NULL OR l.ngay_lam = @ngay_lam)
          AND (@ca_lam_id = '' OR l.ca_lam_id = @ca_lam_id);
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

/* ============================================================
   13. STORED PROCEDURE - KHUYEN MAI (NEW)
   ============================================================ */
CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_create]
(@khuyen_mai_id  varchar(50),
 @ma_khuyen_mai  varchar(30),
 @ten_khuyen_mai nvarchar(150),
 @loai_giam      nvarchar(30),
 @gia_tri_giam   float,
 @ngay_bat_dau   datetime,
 @ngay_ket_thuc  datetime,
 @trang_thai     nvarchar(30)
)
AS
BEGIN
    INSERT INTO [khuyen_mai] (khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai)
    VALUES (@khuyen_mai_id, @ma_khuyen_mai, @ten_khuyen_mai, @loai_giam, @gia_tri_giam, @ngay_bat_dau, @ngay_ket_thuc, @trang_thai);
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_update]
(@khuyen_mai_id  varchar(50),
 @ma_khuyen_mai  varchar(30),
 @ten_khuyen_mai nvarchar(150),
 @loai_giam      nvarchar(30),
 @gia_tri_giam   float,
 @ngay_bat_dau   datetime,
 @ngay_ket_thuc  datetime,
 @trang_thai     nvarchar(30)
)
AS
BEGIN
    UPDATE [khuyen_mai] SET
        ma_khuyen_mai = @ma_khuyen_mai,
        ten_khuyen_mai = @ten_khuyen_mai,
        loai_giam = @loai_giam,
        gia_tri_giam = @gia_tri_giam,
        ngay_bat_dau = @ngay_bat_dau,
        ngay_ket_thuc = @ngay_ket_thuc,
        trang_thai = @trang_thai
    WHERE khuyen_mai_id = @khuyen_mai_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_delete]
(@khuyen_mai_id varchar(50))
AS
BEGIN
    DELETE FROM [khuyen_mai] WHERE khuyen_mai_id = @khuyen_mai_id;
    SELECT '';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_get_by_id]
(@khuyen_mai_id varchar(50))
AS
BEGIN
    SELECT khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai
    FROM [khuyen_mai]
    WHERE khuyen_mai_id = @khuyen_mai_id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_all]
AS
BEGIN
    SELECT khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai
    FROM [khuyen_mai]
    ORDER BY ngay_bat_dau DESC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_get_by_code]
(@ma_khuyen_mai varchar(30))
AS
BEGIN
    SELECT khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai
    FROM [khuyen_mai]
    WHERE ma_khuyen_mai = @ma_khuyen_mai;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_khuyen_mai_search]
(@page_index     INT,
 @page_size      INT,
 @ten_khuyen_mai nvarchar(150),
 @trang_thai     nvarchar(30)
)
AS
BEGIN
    DECLARE @RecordCount BIGINT;
    IF (@page_size <> 0)
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY ngay_bat_dau DESC)) AS RowNumber,
               khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai
        INTO #Results1
        FROM [khuyen_mai]
        WHERE (@ten_khuyen_mai = '' OR ten_khuyen_mai LIKE N'%' + @ten_khuyen_mai + N'%')
          AND (@trang_thai = '' OR trang_thai = @trang_thai);
        SELECT @RecordCount = COUNT(*) FROM #Results1;
        SELECT *, @RecordCount AS RecordCount
        FROM #Results1
        WHERE ROWNUMBER BETWEEN (@page_index - 1) * @page_size + 1 AND (((@page_index - 1) * @page_size + 1) + @page_size) - 1
              OR @page_index = -1;
        DROP TABLE #Results1;
    END
    ELSE
    BEGIN
        SET NOCOUNT ON;
        SELECT (ROW_NUMBER() OVER (ORDER BY ngay_bat_dau DESC)) AS RowNumber,
               khuyen_mai_id, ma_khuyen_mai, ten_khuyen_mai, loai_giam, gia_tri_giam, ngay_bat_dau, ngay_ket_thuc, trang_thai
        INTO #Results2
        FROM [khuyen_mai]
        WHERE (@ten_khuyen_mai = '' OR ten_khuyen_mai LIKE N'%' + @ten_khuyen_mai + N'%')
          AND (@trang_thai = '' OR trang_thai = @trang_thai);
        SELECT @RecordCount = COUNT(*) FROM #Results2;
        SELECT *, @RecordCount AS RecordCount FROM #Results2;
        DROP TABLE #Results2;
    END
END
GO

/* ============================================================
   14. DU LIEU MAU
   ============================================================ */

-- Nhom do uong
IF NOT EXISTS (SELECT 1 FROM item_group)
BEGIN
    INSERT [dbo].[item_group] ([parent_item_group_id], [item_group_id], [item_group_name], [seq_num], [url]) VALUES
    (NULL, N'g0000000-0000-0000-0000-000000000001', N'CÃ  phÃª', 1, NULL),
    (NULL, N'g0000000-0000-0000-0000-000000000002', N'TrÃ  & TrÃ  sá»¯a', 2, NULL),
    (NULL, N'g0000000-0000-0000-0000-000000000003', N'ÄÃ¡ xay & Sinh tá»‘', 3, NULL),
    (NULL, N'g0000000-0000-0000-0000-000000000004', N'BÃ¡nh ngá»t', 4, NULL);
END
GO

-- Mon uong
IF NOT EXISTS (SELECT 1 FROM item)
BEGIN
    INSERT [dbo].[item] ([item_id], [item_group_id], [item_name], [item_image], [item_price]) VALUES
    (N'i0000000-0000-0000-0000-000000000001', N'g0000000-0000-0000-0000-000000000001', N'Espresso',      N'assets/img/product/espresso.jpg',      25000),
    (N'i0000000-0000-0000-0000-000000000002', N'g0000000-0000-0000-0000-000000000001', N'Americano',     N'assets/img/product/americano.jpg',     29000),
    (N'i0000000-0000-0000-0000-000000000003', N'g0000000-0000-0000-0000-000000000001', N'CÃ  phÃª sá»¯a',    N'assets/img/product/ca-phe-sua.jpg',    29000),
    (N'i0000000-0000-0000-0000-000000000004', N'g0000000-0000-0000-0000-000000000001', N'Báº¡c xá»‰u',       N'assets/img/product/bac-xiu.jpg',       32000),
    (N'i0000000-0000-0000-0000-000000000005', N'g0000000-0000-0000-0000-000000000001', N'Cappuccino',    N'assets/img/product/cappuccino.jpg',    35000),
    (N'i0000000-0000-0000-0000-000000000006', N'g0000000-0000-0000-0000-000000000002', N'TrÃ  Ä‘Ã o cam sáº£',N'assets/img/product/tra-dao-cam-sa.jpg', 45000),
    (N'i0000000-0000-0000-0000-000000000007', N'g0000000-0000-0000-0000-000000000002', N'TrÃ  sá»¯a trÃ¢n chÃ¢u', N'assets/img/product/tra-sua-tran-chau.jpg', 39000),
    (N'i0000000-0000-0000-0000-000000000008', N'g0000000-0000-0000-0000-000000000003', N'ÄÃ¡ xay cÃ  phÃª', N'assets/img/product/da-xay-ca-phe.jpg',  45000),
    (N'i0000000-0000-0000-0000-000000000009', N'g0000000-0000-0000-0000-000000000003', N'Sinh tá»‘ xoÃ i',  N'assets/img/product/sinh-to-xoai.jpg',   42000),
    (N'i0000000-0000-0000-0000-000000000010', N'g0000000-0000-0000-0000-000000000004', N'BÃ¡nh croissant',N'assets/img/product/croissant.jpg',      35000),
    (N'i0000000-0000-0000-0000-000000000011', N'g0000000-0000-0000-0000-000000000004', N'BÃ¡nh tiramisu', N'assets/img/product/tiramisu.jpg',       55000);
END
GO

-- ==========================================
-- D? LI?U M?U (S? d?ng GUID chu?n cho t?t c? ID)
-- ==========================================
DECLARE @AdminId VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Staff1Id VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Staff2Id VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @ItemGroup1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @ItemGroup2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @Item1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Item2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Item3 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Item4 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @Ban1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Ban2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Ban3 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @Ban4 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @NguyenLieu1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @NguyenLieu2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @NguyenLieu3 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @CaLam1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @CaLam2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @CaLam3 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @LichLam1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @LichLam2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @KhuyenMai1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @KhuyenMai2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @KhuyenMai3 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @News1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @News2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

DECLARE @HoaDon1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @CTHD1 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));
DECLARE @CTHD2 VARCHAR(50) = CAST(NEWID() AS VARCHAR(50));

-- Nhan vien / quan tri
IF NOT EXISTS (SELECT 1 FROM [user])
BEGIN
    INSERT [dbo].[user] ([user_id], [hoten], [ngaysinh], [diachi], [gioitinh], [email], [taikhoan], [matkhau], [role], [image_url]) VALUES
    (@AdminId, N'Admin',       CAST(N'1990-01-01' AS Date), N'Hung Yï¿½n', N'Nam',  N'admin@bancoffee.vn', N'admin', N'admin',   N'Admin', NULL),
    (@Staff1Id, N'Nguyá»…n VÄƒn Báº£n', CAST(N'2002-05-20' AS Date), N'Hï¿½ Ná»¯i',  N'Nam',  N'ban@bancoffee.vn',   N'ban',   N'123456',  N'Staff', NULL),
    (@Staff2Id, N'Tráº§n Thá»‹ Pha',  CAST(N'2003-09-15' AS Date), N'Hung Yï¿½n', N'Ná»¯', N'pha@bancoffee.vn',   N'pha',   N'123456',  N'Staff', NULL);
END

IF NOT EXISTS (SELECT 1 FROM customer)
BEGIN
    INSERT [dbo].[customer] ([customer_email], [customer_password]) VALUES
    (N'khach1@gmail.com', N'123456'),
    (N'khach2@gmail.com', N'123456');
END

IF NOT EXISTS (SELECT 1 FROM item_group)
BEGIN
    INSERT [dbo].[item_group] ([item_group_id], [item_group_name], [parent_item_group_id], [seq_num], [url]) VALUES
    (@ItemGroup1, N'CÃ  phÃª truyá»n thá»‘ng', NULL, 1, N'ca-phe-truyen-thong'),
    (@ItemGroup2, N'TrÃ  trÃ¡i cÃ¢y', NULL, 2, N'tra-trai-cay');
END

IF NOT EXISTS (SELECT 1 FROM item)
BEGIN
    INSERT [dbo].[item] ([item_id], [item_group_id], [item_name], [item_image], [item_price]) VALUES
    (@Item1, @ItemGroup1, N'CÃ  phÃª den Ä‘Ã¡', NULL, 25000),
    (@Item2, @ItemGroup1, N'CÃ  phÃª sá»¯a Ä‘Ã¡', NULL, 29000),
    (@Item3, @ItemGroup1, N'Báº¡c xá»‰u', NULL, 32000),
    (@Item4, @ItemGroup2, N'TrÃ  dÃ o cam sáº£', NULL, 45000);
END

IF NOT EXISTS (SELECT 1 FROM news)
BEGIN
    INSERT [dbo].[news] ([news_id], [title], [content_news], [created_date]) VALUES
    (@News1, N'Khuyáº¿n mÃ£i khai truong', N'Giáº£m 20% toÃ n bá»™ menu trong tuáº§n khai trÆ°Æ¡ng.', CAST(N'2026-09-10T08:00:00' AS DateTime)),
    (@News2, N'Mua 1 táº·ng 1 giÃ¡ vÃ ng',  N'GiÃ¡ vÃ ng 14h - 16h hÃ ng ngÃ y.',   CAST(N'2026-09-12T09:30:00' AS DateTime));
END

IF NOT EXISTS (SELECT 1 FROM ban)
BEGIN
    INSERT [dbo].[ban] ([ban_id], [ten_ban], [khu_vuc], [so_cho], [trang_thai]) VALUES
    (@Ban1, N'BÃ n 1', N'Táº§ng 1', 2, N'Trá»‘ng'),
    (@Ban2, N'BÃ n 2', N'Táº§ng 1', 4, N'Trá»‘ng'),
    (@Ban3, N'BÃ n 3', N'Táº§ng 1', 4, N'Äang phá»¥c vá»¥'),
    (@Ban4, N'BÃ n 4', N'Táº§ng 2', 2, N'Trá»‘ng');
END

IF NOT EXISTS (SELECT 1 FROM nguyen_lieu)
BEGIN
    INSERT [dbo].[nguyen_lieu] ([nguyen_lieu_id], [ten_nguyen_lieu], [don_vi], [so_luong_ton], [gia_nhap], [ngay_nhap_cuoi]) VALUES
    (@NguyenLieu1, N'CÃ  phÃª háº¡t', N'kg', 50, 150000, CAST(N'2026-09-01T08:00:00' AS DateTime)),
    (@NguyenLieu2, N'Sá»¯a Ä‘áº·c', N'lon', 100, 25000, CAST(N'2026-09-02T08:00:00' AS DateTime)),
    (@NguyenLieu3, N'TrÃ  den', N'kg', 15, 120000, CAST(N'2026-09-05T08:00:00' AS DateTime));
END

IF NOT EXISTS (SELECT 1 FROM ca_lam)
BEGIN
    INSERT [dbo].[ca_lam] ([ca_lam_id], [ten_ca], [gio_bat_dau], [gio_ket_thuc]) VALUES
    (@CaLam1, N'Ca sÃ¡ng', CAST('06:00:00' AS Time), CAST('14:00:00' AS Time)),
    (@CaLam2, N'Ca chiá»u', CAST('14:00:00' AS Time), CAST('22:00:00' AS Time)),
    (@CaLam3, N'Ca tá»‘i', CAST('22:00:00' AS Time), CAST('06:00:00' AS Time));
END

IF NOT EXISTS (SELECT 1 FROM lich_lam_viec)
BEGIN
    INSERT [dbo].[lich_lam_viec] ([lich_id], [user_id], [ca_lam_id], [ngay_lam], [ghi_chu]) VALUES
    (@LichLam1, @Staff1Id, @CaLam1, CAST(N'2026-09-30' AS Date), NULL),
    (@LichLam2, @Staff2Id, @CaLam2, CAST(N'2026-09-30' AS Date), NULL);
END

IF NOT EXISTS (SELECT 1 FROM khuyen_mai)
BEGIN
    INSERT [dbo].[khuyen_mai] ([khuyen_mai_id], [ma_khuyen_mai], [ten_khuyen_mai], [loai_giam], [gia_tri_giam], [ngay_bat_dau], [ngay_ket_thuc], [trang_thai]) VALUES
    (@KhuyenMai1, N'NEW10', N'Giáº£m giÃ¡ thÃ nh viÃªn má»›i', N'Pháº§n trÄƒm', 10, CAST(N'2026-09-01T00:00:00' AS DateTime), CAST(N'2026-10-01T00:00:00' AS DateTime), N'Hoáº¡t Ä‘á»™ng'),
    (@KhuyenMai2, N'GIAM20K', N'Giáº£m 20k Ä‘Æ¡n tá»« 100k', N'Sá»‘ tiá»n', 20000, CAST(N'2026-09-15T00:00:00' AS DateTime), CAST(N'2026-09-30T23:59:59' AS DateTime), N'Hoáº¡t Ä‘á»™ng'),
    (@KhuyenMai3, N'HETHAN', N'MÃ£ Ä‘Ã£ háº¿t háº¡n', N'Pháº§n trÄƒm', 50, CAST(N'2026-08-01T00:00:00' AS DateTime), CAST(N'2026-08-31T23:59:59' AS DateTime), N'Háº¿t háº¡n');
END

IF NOT EXISTS (SELECT 1 FROM hoa_don)
BEGIN
    INSERT [dbo].[hoa_don] ([ma_hoa_don], [ho_ten], [dia_chi], [ngay_tao], [tong_tien], [giam_gia], [thanh_tien], [hinh_thuc_thanh_toan], [ghi_chu], [user_id], [ban_id], [khuyen_mai_id]) VALUES
    (@HoaDon1, N'KhÃ¡ch láº» 01', NULL, CAST(N'2026-09-13T08:15:00' AS DateTime), 61000, 0, 61000, N'Tiá»n máº·t', NULL, @Staff1Id, @Ban3, NULL);

    INSERT [dbo].[chi_tiet_hoa_don] ([ma_chi_tiet], [ma_hoa_don], [item_id], [so_luong], [don_gia]) VALUES
    (@CTHD1, @HoaDon1, @Item2, 1, 29000),
    (@CTHD2, @HoaDon1, @Item3, 1, 32000);
END
GO







-- Cong thuc (Recipe mapping Item to NguyenLieu)
IF OBJECT_ID(N'dbo.cong_thuc', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[cong_thuc](
    [cong_thuc_id] [varchar](50) NOT NULL,
    [item_id] [varchar](50) NOT NULL,
    [nguyen_lieu_id] [varchar](50) NOT NULL,
    [so_luong_tieu_hao] [float] NOT NULL,
    CONSTRAINT [PK_cong_thuc] PRIMARY KEY CLUSTERED ([cong_thuc_id] ASC)
) ON [PRIMARY]
END
GO
