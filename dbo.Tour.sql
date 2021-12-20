CREATE TABLE [dbo].[Tour] (
    [MaTour]     VARCHAR (50)    NOT NULL,
    [TenTour]    NVARCHAR (1000) NULL,
    [DacDiem]    NVARCHAR (MAX)  NULL,
    [MaLoaiHinh] VARCHAR (50)    NOT NULL,
    [AnhTour] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_Tour] PRIMARY KEY CLUSTERED ([MaTour] ASC),
    CONSTRAINT [FK_Tour_LoaiHinhDuLich] FOREIGN KEY ([MaLoaiHinh]) REFERENCES [dbo].[LoaiHinhDuLich] ([MaLoaiHinh])
);

