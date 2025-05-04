using System;
using OpenTK.Graphics.OpenGL;

namespace Goopify
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            this.Name = name;
            this.Location = location;
            this.Type = type;
        }
    }

    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            this.Name = name;
            this.Location = location;
            this.Type = type;
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        private bool disposed;

        public readonly int ShaderProgramHandle;
        public readonly int VertexShaderHandle;
        public readonly int PixelShaderHandle;

        private readonly ShaderUniform[] uniforms;
        private readonly ShaderAttribute[] attributes;

        public ShaderProgram(string vertexShaderCode, string pixelShaderCode)
        {
            this.disposed = false;

            if (!ShaderProgram.CompileVertexShader(vertexShaderCode, out this.VertexShaderHandle, out string vertexShaderCompileError))
            {
                throw new ArgumentException(vertexShaderCompileError);
            }

            if (!ShaderProgram.CompilePixelShader(pixelShaderCode, out this.PixelShaderHandle, out string pixelShaderCompileError))
            {
                throw new ArgumentException(pixelShaderCompileError);
            }

            this.ShaderProgramHandle = ShaderProgram.CreateLinkProgram(this.VertexShaderHandle, this.PixelShaderHandle);

            this.uniforms = ShaderProgram.CreateUniformList(this.ShaderProgramHandle);
            this.attributes = ShaderProgram.CreateAttributeList(this.ShaderProgramHandle);
        }

        ~ShaderProgram()
        {
            //this.Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            GL.DeleteShader(this.VertexShaderHandle);
            GL.DeleteShader(this.PixelShaderHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.ShaderProgramHandle);

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniformList()
        {
            ShaderUniform[] result = new ShaderUniform[this.uniforms.Length];
            Array.Copy(this.uniforms, result, this.uniforms.Length);
            return result;
        }

        public ShaderAttribute[] GetAttributeList()
        {
            ShaderAttribute[] result = new ShaderAttribute[this.attributes.Length];
            Array.Copy(this.attributes, result, this.attributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (!this.GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException("Name was not found.");
            }

            if (uniform.Type != ActiveUniformType.Float)
            {
                throw new ArgumentException("Uniform type is not float.");
            }

            GL.UseProgram(this.ShaderProgramHandle);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2)
        {
            if (!this.GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException("Name was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec2)
            {
                throw new ArgumentException("Uniform type is not FloatVec2.");
            }

            GL.UseProgram(this.ShaderProgramHandle);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(this.ShaderProgramHandle, name);
            GL.Uniform1(location, value);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new ShaderUniform();

            for (int i = 0; i < this.uniforms.Length; i++)
            {
                uniform = this.uniforms[i];

                if (name == uniform.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;

            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckCompileErrors(vertexShaderHandle, "VERTEX");

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
            if (vertexShaderInfo != String.Empty)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }

            return true;
        }

        public static bool CompilePixelShader(string pixelShaderCode, out int pixelShaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;

            pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);
            CheckCompileErrors(pixelShaderHandle, "FRAGMENT");

            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
            if (pixelShaderInfo != String.Empty)
            {
                errorMessage = pixelShaderInfo;
                return false;
            }

            return true;
        }

        public static int CreateLinkProgram(int vertexShaderHandle, int pixelShaderHandle)
        {
            int shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(shaderProgramHandle);
            CheckLinkingErrors(shaderProgramHandle);
            CheckCompileErrors(shaderProgramHandle, "PROGRAM");

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, pixelShaderHandle);

            return shaderProgramHandle;
        }

        public static ShaderUniform[] CreateUniformList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            ShaderUniform[] uniforms = new ShaderUniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(shaderProgramHandle, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int location = GL.GetUniformLocation(shaderProgramHandle, name);
                uniforms[i] = new ShaderUniform(name, location, type);
            }

            return uniforms;
        }

        public static ShaderAttribute[] CreateAttributeList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            ShaderAttribute[] attributes = new ShaderAttribute[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(shaderProgramHandle, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int location = GL.GetAttribLocation(shaderProgramHandle, name);
                attributes[i] = new ShaderAttribute(name, location, type);
            }

            return attributes;
        }

        private static void CheckCompileErrors(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}");
            }
        }

        public static void CheckLinkingErrors(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR\n{infoLog}");
            }
        }

    }
}
