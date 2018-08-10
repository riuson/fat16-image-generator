using FSImageGenerator.Classes.Helpers;
using System;
using System.Collections.Generic;

namespace FSImageGenerator.Classes.System {
    class BootSector : IPart {
        public IEnumerable<Byte> GetBytes() {
            var result = new Byte[Math.Max(1u, this.ReservedSectors) * 512u];
            Converter.GetBytes(this.JumpCode, 3).CopyTo(result, 0x00);
            Converter.GetBytes(this.OemID, 8).CopyTo(result, 0x03);
            Converter.GetBytes(this.BytesPerSector).CopyTo(result, 0x0b);
            result[0x0d] = this.SectorsPerCluster;
            Converter.GetBytes(this.ReservedSectors).CopyTo(result, 0x0e);
            result[0x10] = this.NumberOfFATs;
            Converter.GetBytes(this.RootEntries).CopyTo(result, 0x11);

            if (this.TotalSectors * this.BytesPerSector > 32000000) {
                Converter.GetBytes((UInt16)0).CopyTo(result, 0x13);
                Converter.GetBytes((UInt32)this.TotalSectors).CopyTo(result, 0x20);
            } else {
                Converter.GetBytes((UInt16)this.TotalSectors).CopyTo(result, 0x13);
                Converter.GetBytes((UInt32)0).CopyTo(result, 0x20);
            }

            result[0x15] = this.MediaDescriptor;
            Converter.GetBytes(this.SectorsPerFat).CopyTo(result, 0x16);
            Converter.GetBytes(this.SectorsPerTrack).CopyTo(result, 0x18);
            Converter.GetBytes(this.Heads).CopyTo(result, 0x1a);
            Converter.GetBytes(this.HiddenSectors).CopyTo(result, 0x1c);
            result[0x24] = this.PhysicalDiskNumber;
            result[0x25] = 0;
            result[0x26] = this.Signature;
            Converter.GetBytes(this.VolumeSerialNumber).CopyTo(result, 0x27);
            Converter.GetBytes(this.VolumeLabel, 11).CopyTo(result, 0x2b);
            Converter.GetBytes(this.SystemID, 8).CopyTo(result, 0x36);
            Converter.GetBytes((UInt16)0xaa55).CopyTo(result, 0x1fe);

            return result;
        }

        /// <summary>
        /// Машинная инструкция для перехода на загрузочный код.
        /// </summary>
        public UInt32 JumpCode { get; } = 0x9058eb;
        /// <summary>
        /// Текстовый идентификатор ОС или файловой системы.
        /// </summary>
        public String OemID { get; set; } = "RIUSON";
        /// <summary>
        /// Количество байт в секторе, обычно 512 (0x200).
        /// </summary>
        public UInt16 BytesPerSector { get; set; } = 512;
        /// <summary>
        /// Количество секторов в кластере.
        /// </summary>
        public Byte SectorsPerCluster { get; set; } = 1;
        /// <summary>
        /// Количество зарезервированных секторов.
        /// </summary>
        public UInt16 ReservedSectors { get; set; } = 1;
        /// <summary>
        /// Количество FAT таблиц.
        /// </summary>
        public Byte NumberOfFATs { get; set; } = 2;
        /// <summary>
        /// Максимальное число 32-байтных элементов корневого каталога.
        /// </summary>
        public UInt16 RootEntries { get; set; } = 512;
        /// <summary>
        /// Общее число секторов на томе.
        /// Может храниться в двух местах:
        /// По адресу 0x13 в виде UInt16, если менее диск менее 32МБ.
        /// По адресу 0x20 в виде UInt32, если более 32МБ.
        /// </summary>
        public UInt32 TotalSectors { get; set; } = 0;
        /// <summary>
        /// Дескриптор носителя (то же, что  первом байте FAT таблицы).
        /// </summary>
        public Byte MediaDescriptor { get; } = 0xf8;
        /// <summary>
        /// Число секторов в одной таблице FAT.
        /// </summary>
        public UInt16 SectorsPerFat { get; set; } = 1;
        /// <summary>
        /// Число секторов на дорожке.
        /// </summary>
        public UInt16 SectorsPerTrack { get; } = 2;
        /// <summary>
        /// Число головок.
        /// </summary>
        public UInt16 Heads { get; } = 1;
        /// <summary>
        /// Число скрытых секторов.
        /// </summary>
        public UInt32 HiddenSectors { get; set; } = 0;
        /// <summary>
        /// Физический номер устройства, присваивается в процессе форматирования (0x80 - первый жёсткий диск в системе).
        /// </summary>
        public Byte PhysicalDiskNumber { get; } = 0x80;
        /// <summary>
        /// Сигнатура расширенного загрузчика.
        /// </summary>
        public Byte Signature { get; } = 0x29;
        /// <summary>
        /// Серийный номер тома, устанавливается при форматировании.
        /// </summary>
        public UInt32 VolumeSerialNumber { get; set; } = 0;
        /// <summary>
        /// Метка тома диска.
        /// </summary>
        public String VolumeLabel { get; set; } = String.Empty;
        /// <summary>
        /// Сивольный код идентификатора файловой системы, FAT16.
        /// </summary>
        public String SystemID { get; set; } = "FAT16";
    }
}
