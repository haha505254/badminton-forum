-- Create database schema for Badminton Forum
-- This is a simplified version for quick setup

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Email" VARCHAR(256) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "DisplayName" VARCHAR(100) NOT NULL,
    "Avatar" TEXT,
    "Bio" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "IsEmailConfirmed" BOOLEAN DEFAULT FALSE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "IsAdmin" BOOLEAN DEFAULT FALSE,
    "PasswordResetToken" TEXT,
    "PasswordResetTokenExpiry" TIMESTAMP WITH TIME ZONE
);

-- Create ForumCategories table
CREATE TABLE IF NOT EXISTS "ForumCategories" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "Description" TEXT,
    "Slug" VARCHAR(100) NOT NULL UNIQUE,
    "Color" VARCHAR(20),
    "Icon" VARCHAR(50),
    "DisplayOrder" INTEGER DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create Posts table
CREATE TABLE IF NOT EXISTS "Posts" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Title" VARCHAR(200) NOT NULL,
    "Content" TEXT NOT NULL,
    "CategoryId" UUID NOT NULL REFERENCES "ForumCategories"("Id") ON DELETE CASCADE,
    "AuthorId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ViewCount" INTEGER DEFAULT 0,
    "IsPinned" BOOLEAN DEFAULT FALSE,
    "IsLocked" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create Replies table
CREATE TABLE IF NOT EXISTS "Replies" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Content" TEXT NOT NULL,
    "PostId" UUID NOT NULL REFERENCES "Posts"("Id") ON DELETE CASCADE,
    "AuthorId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ParentReplyId" UUID REFERENCES "Replies"("Id") ON DELETE CASCADE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create PostLikes table
CREATE TABLE IF NOT EXISTS "PostLikes" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "PostId" UUID NOT NULL REFERENCES "Posts"("Id") ON DELETE CASCADE,
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE("PostId", "UserId")
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_posts_category ON "Posts"("CategoryId");
CREATE INDEX IF NOT EXISTS idx_posts_author ON "Posts"("AuthorId");
CREATE INDEX IF NOT EXISTS idx_replies_post ON "Replies"("PostId");
CREATE INDEX IF NOT EXISTS idx_replies_author ON "Replies"("AuthorId");
CREATE INDEX IF NOT EXISTS idx_postlikes_post ON "PostLikes"("PostId");
CREATE INDEX IF NOT EXISTS idx_postlikes_user ON "PostLikes"("UserId");

-- Insert default categories
INSERT INTO "ForumCategories" ("Name", "Description", "Slug", "Color", "Icon", "DisplayOrder") VALUES
('技術討論', '分享羽球技術、戰術和訓練心得', 'technical-discussion', '#3B82F6', 'mdi-school', 1),
('裝備評測', '球拍、球鞋、球線等裝備的評測與推薦', 'equipment-review', '#10B981', 'mdi-tennis', 2),
('比賽資訊', '各類羽球比賽資訊、賽程和結果', 'competition-info', '#F59E0B', 'mdi-trophy', 3),
('找球友', '尋找同好一起打球、組隊參賽', 'find-partners', '#EC4899', 'mdi-account-group', 4)
ON CONFLICT (slug) DO NOTHING;