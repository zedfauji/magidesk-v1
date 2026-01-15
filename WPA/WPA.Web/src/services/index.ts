import { MockAuthService } from "./mock/MockAuthService";
import { MockMenuService } from "./mock/MockMenuService";
import { MockOrderService } from "./mock/MockOrderService";
import { MockTableService } from "./mock/MockTableService";

import { HttpAuthService } from "./http/HttpAuthService";
import { HttpMenuService } from "./http/HttpMenuService";
import { HttpOrderService } from "./http/HttpOrderService";
import { HttpTableService } from "./http/HttpTableService";

import type { IAuthService, IMenuService, IOrderService, ITableService } from "./interfaces";

// Default to mocks if VITE_USE_MOCKS is not explicitly 'false'
const useMocks = import.meta.env.VITE_USE_MOCKS !== 'false';

console.log(`[WPA] Service Mode: ${useMocks ? 'MOCK' : 'HTTP (Real Backend)'}`);

export const authService: IAuthService = useMocks ? new MockAuthService() : new HttpAuthService();
export const tableService: ITableService = useMocks ? new MockTableService() : new HttpTableService();
export const menuService: IMenuService = useMocks ? new MockMenuService() : new HttpMenuService();
export const orderService: IOrderService = useMocks ? new MockOrderService() : new HttpOrderService();
