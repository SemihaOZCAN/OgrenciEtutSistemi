Create procedure  Etut
as
SELECT 
    ID, 
    DERSAD, 
    (TBLOGRETMEN.AD + ' ' + TBLOGRETMEN.SOYAD) AS '��retmen', 
    (TBLOGRENCI.AD + ' ' + TBLOGRENCI.SOYAD) AS '��renci', 
    TARIH, 
    SAAT, 
    DURUM 
FROM TBLETUT
LEFT JOIN TBLDERS ON TBLETUT.DERSID = TBLDERS.DERSID
LEFT JOIN TBLOGRETMEN ON TBLETUT.OGRETMENID = TBLOGRETMEN.OGRTID
LEFT JOIN TBLOGRENCI ON TBLETUT.OGRENCIID = TBLOGRENCI.OGRID;

