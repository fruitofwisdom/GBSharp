﻿namespace GBSharp
{
	internal partial class CPU
	{
		void HandleOpcode(byte instruction)
		{
			// Ensure ROM data was loaded.
			if (ROM.Instance.Data is null)
			{
				return;
			}

			switch (instruction)
			{
				case 0x00:      // NOP
					{
						PrintOpcode(instruction, "NOP");
						PC++;
						Cycles++;
					}
					break;

				case 0x01:      // LD BC, d16
					{
						C = ROM.Instance.Data[PC + 1];
						B = ROM.Instance.Data[PC + 2];
						ushort d16 = (ushort)((B << 8) + C);
						PrintOpcode(instruction, $"LD BC, 0x{d16:X4}");
						PC += 3;
						Cycles += 3;
					}
					break;

				case 0x04:      // INC B
					{
						B++;
						Z = B == 0;
						N = false;
						// TODO: H?
						PC++;
						Cycles++;
					}
					break;

				case 0x05:      // DEC B
					{
						B--;
						Z = B == 0;
						N = true;
						// TODO: H?
						PC++;
						Cycles++;
					}
					break;

				case 0x06:      // LD B, d8
					{
						byte d8 = ROM.Instance.Data[PC + 1];
						B = d8;
						PrintOpcode(instruction, $"LD B, 0x{d8:X2}");
						PC += 2;
						Cycles += 2;
					}
					break;

				case 0x0B:      // DEC BC
					{
						ushort bc = (ushort)((B << 8) + C);
						bc--;
						B = (byte)((bc & 0xFF00) >> 8);
						C = (byte)(bc & 0x00FF);
						PrintOpcode(instruction, "DEC BC");
						PC++;
						Cycles += 2;
					}
					break;

				case 0x18:      // JR s8
					{
						sbyte s8 = (sbyte)(ROM.Instance.Data[PC + 1] + 2);
						ushort newPC = (ushort)(PC + s8);
						PrintOpcode(instruction, $"JR 0x{newPC:X4}");
						PC = newPC;
						Cycles += 3;
					}
					break;

				case 0x21:      // LD HL, d16
					{
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = (ushort)(ROM.Instance.Data[PC + 2] << 8);
						ushort d16 = (ushort)(higher + lower);
						HL = d16;
						PrintOpcode(instruction, $"LD HL, 0x{d16:X4}");
						PC += 3;
						Cycles += 3;
					}
					break;

				case 0x23:      // INC HL
					{
						HL++;
						PrintOpcode(instruction, "INC HL");
						PC++;
						Cycles += 2;
					}
					break;

				case 0x30:      // JR NC, s8
					{
						sbyte s8 = (sbyte)(ROM.Instance.Data[PC + 1] + 2);
						ushort newPC = (ushort)(PC + s8);
						PrintOpcode(instruction, $"JR NC, 0x{newPC:X4}");
						if (!CY)
						{
							PC = newPC;
							Cycles += 3;
						}
						else
						{
							PC += 2;
							Cycles += 2;
						}
					}
					break;

				case 0x31:      // LD SP, d16
					{
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = (ushort)(ROM.Instance.Data[PC + 2] << 8);
						ushort d16 = (ushort)(higher + lower);
						SP = d16;
						PrintOpcode(instruction, $"LD SP, 0x{d16:X4}");
						PC += 3;
						Cycles += 3;
					}
					break;

				case 0x38:      // JR C, s8
					{
						sbyte s8 = (sbyte)(ROM.Instance.Data[PC + 1] + 2);
						ushort newPC = (ushort)(PC + s8);
						PrintOpcode(instruction, $"JR C, 0x{newPC:X4}");
						if (CY)
						{
							PC = newPC;
							Cycles += 3;
						}
						else
						{
							PC += 2;
							Cycles += 2;
						}
					}
					break;

				case 0x3E:      // LD A, d8
					{
						byte d8 = ROM.Instance.Data[PC + 1];
						A = d8;
						PrintOpcode(instruction, $"LD A, 0x{d8:X2}");
						PC += 2;
						Cycles += 2;
					}
					break;

				case 0x57:      // LD D, A
					{
						D = A;
						PrintOpcode(instruction, "LD D, A");
						PC++;
						Cycles++;
					}
					break;

					/*
				case 0x66:		// LD H, (HL)
					{
						ushort memory = (ushort)(Memory.Instance.Read(HL) << 8);
						// NOTE: H is the higher byte of register HL.
						HL = memory;
						PrintOpcode(instruction, ...);
						PC++;
						Cycles += 2;
					}
					break;
					*/

				case 0x72:      // LD (HL), D
					{
						Memory.Instance.Write(HL, D);
						PrintOpcode(instruction, "LD (HL), D");
						PC++;
						Cycles += 2;
					}
					break;

				case 0x78:      // LD A, B
					{
						A = B;
						PrintOpcode(instruction, "LD A, B");
						PC++;
						Cycles++;
					}
					break;

				case 0xB1:      // OR C
					{
						A |= C;
						Z = A == 0x00;
						N = false;
						H = false;
						CY = false;
						PrintOpcode(instruction, "OR C");
						PC++;
						Cycles++;
					}
					break;

				case 0xC3:      // JP a16
					{
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = (ushort)(ROM.Instance.Data[PC + 2] << 8);
						ushort a16 = (ushort)(higher + lower);
						PrintOpcode(instruction, $"JP 0x{a16:X4}");
						PC = a16;
						Cycles += 4;
					}
					break;

				case 0xC8:      // RET Z
					{
						PrintOpcode(instruction, "RET Z");
						if (Z)
						{
							byte lower = Memory.Instance.Read(SP);
							SP++;
							ushort higher = (ushort)(Memory.Instance.Read(SP) << 8);
							SP++;
							PC = (ushort)(higher + lower);
							Cycles += 5;
						}
						else
						{
							PC++;
							Cycles += 2;
						}
					}
					break;

				case 0xC9:      // RET
					{
						byte lower = Memory.Instance.Read(SP);
						SP++;
						ushort higher = (ushort)(Memory.Instance.Read(SP) << 8);
						SP++;
						PrintOpcode(instruction, "RET");
						PC = (ushort)(higher + lower);
						Cycles += 4;
					}
					break;

				case 0xCD:      // CALL a16
					{
						ushort nextPC = (ushort)(PC + 3);
						byte pcHigher = (byte)((nextPC & 0xFF00) >> 8);
						Memory.Instance.Write(SP - 1, pcHigher);
						byte pcLower = (byte)(nextPC & 0x00FF);
						Memory.Instance.Write(SP - 2, pcLower);
						SP -= 2;
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = (ushort)(ROM.Instance.Data[PC + 2] << 8);
						ushort a16 = (ushort)(higher + lower);
						PrintOpcode(instruction, $"CALL 0x{a16:X4}");
						PC = a16;
						Cycles += 6;
					}
					break;

				case 0xD1:      // POP DE
					{
						E = Memory.Instance.Read(SP);
						SP++;
						D = Memory.Instance.Read(SP);
						SP++;
						PrintOpcode(instruction, "POP DE");
						PC++;
						Cycles += 3;
					}
					break;

				case 0xD5:      // PUSH DE
					{
						Memory.Instance.Write(SP - 1, D);
						Memory.Instance.Write(SP - 2, E);
						SP -= 2;
						PrintOpcode(instruction, "PUSH DE");
						PC++;
						Cycles += 4;
					}
					break;

				case 0xE0:      // LD (a8), A
					{
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = 0xFF00;
						Memory.Instance.Write(higher + lower, A);
						PrintOpcode(instruction, $"LD (0x{lower:X2}), A");
						PC += 2;
						Cycles += 3;
					}
					break;

				case 0xE6:      // AND d8
					{
						byte d8 = ROM.Instance.Data[PC + 1];
						A &= d8;
						Z = A == 0;
						N = false;
						H = true;
						CY = false;
						PrintOpcode(instruction, $"AND 0x{d8:2}");
						PC += 2;
						Cycles += 3;
					}
					break;

				case 0xF0:      // LD A, (a8)
					{
						byte lower = ROM.Instance.Data[PC + 1];
						ushort higher = 0xFF00;
						A = Memory.Instance.Read(higher + lower);
						PrintOpcode(instruction, $"LD A, (0x{lower:X2})");
						PC += 2;
						Cycles += 3;
					}
					break;

				case 0xF3:      // DI
					{
						IME = false;
						PrintOpcode(instruction, "DI");
						PC++;
						Cycles++;
					}
					break;

				case 0xFE:      // CP d8
					{
						byte d8 = ROM.Instance.Data[PC + 1];
						int cp = A - d8;
						Z = cp == 0;
						// TODO: H?
						CY = cp < 0;
						PrintOpcode(instruction, $"CP 0x{d8:X2}");
						PC += 2;
						Cycles += 2;
					}
					break;

				default:
					{
						MainForm.PrintDebugMessage($"Unimplemented opcode: 0x{instruction:X2}!\n");
						MainForm.Pause();
					}
					break;
			}
		}
	}
}
