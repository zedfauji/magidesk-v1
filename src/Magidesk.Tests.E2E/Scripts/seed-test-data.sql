-- Seed Test Data Script
-- Seeds minimum required data for E2E tests
-- This script is executed after database reset to ensure baseline configuration exists

-- Seed admin user if not exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM users WHERE username = 'admin') THEN
        INSERT INTO users (
            id,
            username,
            password_hash,
            first_name,
            last_name,
            role_id,
            is_active,
            created_at,
            updated_at,
            version
        )
        VALUES (
            gen_random_uuid(),
            'admin',
            -- BCrypt hash for 'admin123' (cost factor 11)
            '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
            'Admin',
            'User',
            (SELECT id FROM roles WHERE name = 'Admin' LIMIT 1),
            true,
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Admin user created with username: admin, password: admin123';
    END IF;
END $$;

-- Seed default terminal if not exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM terminals WHERE terminal_number = 1) THEN
        INSERT INTO terminals (
            id,
            terminal_number,
            name,
            is_active,
            created_at,
            updated_at,
            version
        )
        VALUES (
            gen_random_uuid(),
            1,
            'Terminal 1',
            true,
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Default terminal created: Terminal 1';
    END IF;
END $$;

-- Seed restaurant configuration if not exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM restaurant_configurations LIMIT 1) THEN
        INSERT INTO restaurant_configurations (
            id,
            restaurant_name,
            tax_rate,
            reduced_tax_rate,
            currency_code,
            created_at,
            updated_at,
            version
        )
        VALUES (
            gen_random_uuid(),
            'Test Restaurant',
            0.10,  -- 10% standard tax rate
            0.05,  -- 5% reduced tax rate
            'USD',
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Restaurant configuration created with standard tax: 10%, reduced tax: 5%';
    END IF;
END $$;

-- Seed default roles if not exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM roles WHERE name = 'Admin') THEN
        INSERT INTO roles (id, name, description, created_at, updated_at, version)
        VALUES (
            gen_random_uuid(),
            'Admin',
            'Administrator with full system access',
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Admin role created';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM roles WHERE name = 'Manager') THEN
        INSERT INTO roles (id, name, description, created_at, updated_at, version)
        VALUES (
            gen_random_uuid(),
            'Manager',
            'Manager with operational access',
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Manager role created';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM roles WHERE name = 'Server') THEN
        INSERT INTO roles (id, name, description, created_at, updated_at, version)
        VALUES (
            gen_random_uuid(),
            'Server',
            'Server with order entry access',
            NOW(),
            NOW(),
            1
        );
        
        RAISE NOTICE 'Server role created';
    END IF;
END $$;
